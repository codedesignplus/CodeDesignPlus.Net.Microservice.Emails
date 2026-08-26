using System.Diagnostics;
using System.Diagnostics.Metrics;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Services;
using CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Options;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Services.PdfGenerator;

/// <summary>
/// Genera PDF a partir de HTML reutilizando un unico navegador por proceso.
/// </summary>
/// <remarks>
/// SE REGISTRA COMO SINGLETON, Y ES OBLIGATORIO. Antes era Scoped y cada peticion hacia
/// Puppeteer.LaunchAsync, es decir, arrancaba un proceso Chromium entero. Eso mato al pod por
/// OOM el 26-ago-2026 (exitCode 137) con un limite de 320Mi: arrancar el navegador es el pico,
/// no el renderizado. Abrir una pestaña en un navegador ya vivo cuesta una fraccion.
/// </remarks>
public sealed class PdfGenerator(IOptions<PdfGeneratorOptions> options, ILogger<PdfGenerator> logger) : IPdfGenerator, IAsyncDisposable
{
    /// <summary>
    /// Nombre del medidor. Hay que declararlo en la configuracion de OpenTelemetry para que estas
    /// metricas salgan del proceso; si no, se emiten y nadie las recoge.
    /// </summary>
    public const string MeterName = "CodeDesignPlus.Emails.PdfGenerator";

    private static readonly Meter Meter = new(MeterName);
    private static readonly Histogram<double> QueueWait = Meter.CreateHistogram<double>("pdf.render.queue.wait", "ms", "Tiempo que una peticion espera un hueco de renderizado.");
    private static readonly Histogram<double> RenderDuration = Meter.CreateHistogram<double>("pdf.render.duration", "ms", "Tiempo de renderizado de un documento.");
    private static readonly Counter<long> Rejected = Meter.CreateCounter<long>("pdf.render.rejected", "1", "Peticiones descartadas por no conseguir hueco a tiempo.");
    private static readonly Counter<long> BrowserLaunches = Meter.CreateCounter<long>("pdf.browser.launches", "1", "Veces que se ha arrancado el navegador.");

    private readonly SemaphoreSlim renderSlots = new(options.Value.MaxConcurrentRenders, options.Value.MaxConcurrentRenders);
    private readonly SemaphoreSlim browserLock = new(1, 1);
    private readonly PdfGeneratorOptions settings = options.Value;

    private IBrowser? browser;
    private int active;

    /// <summary>
    /// Genera un PDF A4 a partir del HTML indicado.
    /// </summary>
    /// <param name="html">Contenido HTML a renderizar.</param>
    /// <param name="cancellationToken">Token de cancelación para detener la operación.</param>
    /// <returns>El contenido binario del PDF generado.</returns>
    /// <exception cref="TimeoutException">Se lanza cuando no se consigue un hueco de renderizado dentro del plazo configurado.</exception>
    public async Task<byte[]> GenerateFromHtmlAsync(string html, CancellationToken cancellationToken)
    {
        var waited = Stopwatch.StartNew();

        // LA COLA ES EL TECHO DE MEMORIA. Quien no cabe espera; quien espera demasiado se rinde.
        // Rendirse es mejor que acumular peticiones vivas hasta que el proceso reviente, que es
        // exactamente lo que pasaba antes cuando cada una traia su propio Chromium.
        if (!await renderSlots.WaitAsync(TimeSpan.FromSeconds(settings.QueueTimeoutSeconds), cancellationToken))
        {
            Rejected.Add(1);

            logger.LogWarning("No render slot available after {Seconds}s with {Active} active renders.", settings.QueueTimeoutSeconds, Volatile.Read(ref active));

            throw new TimeoutException($"No render slot available after {settings.QueueTimeoutSeconds}s.");
        }

        waited.Stop();
        QueueWait.Record(waited.Elapsed.TotalMilliseconds);

        var running = Interlocked.Increment(ref active);
        var rendered = Stopwatch.StartNew();

        try
        {
            if (running > 1)
            {
                logger.LogInformation("Rendering with {Active} concurrent renders (limit {Limit}).", running, settings.MaxConcurrentRenders);
            }

            return await RenderAsync(html, cancellationToken);
        }
        finally
        {
            rendered.Stop();
            RenderDuration.Record(rendered.Elapsed.TotalMilliseconds);

            Interlocked.Decrement(ref active);
            renderSlots.Release();
        }
    }

    /// <summary>
    /// Abre una pestaña en el navegador compartido y produce el PDF.
    /// </summary>
    private async Task<byte[]> RenderAsync(string html, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(settings.RenderTimeoutSeconds));

        var instance = await GetBrowserAsync(timeout.Token);

        // La pestaña SI es por peticion, y debe serlo: es lo que aisla un render de otro. Lo que
        // no puede ser por peticion es el navegador.
        await using var page = await instance.NewPageAsync();

        var navigation = new NavigationOptions
        {
            WaitUntil = [WaitUntilNavigation.Networkidle0],
            Timeout = settings.RenderTimeoutSeconds * 1000
        };

        await page.SetContentAsync(html, navigation);

        return await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "20mm",
                Bottom = "20mm",
                Left = "15mm",
                Right = "15mm"
            }
        });
    }

    /// <summary>
    /// Devuelve el navegador compartido, arrancandolo si no existe o si murio.
    /// </summary>
    private async Task<IBrowser> GetBrowserAsync(CancellationToken cancellationToken)
    {
        var current = browser;

        if (current is { IsClosed: false })
        {
            return current;
        }

        await browserLock.WaitAsync(cancellationToken);

        try
        {
            // Se vuelve a mirar dentro del cerrojo: si dos peticiones entraron a la vez, la
            // segunda debe encontrarlo ya arrancado y no lanzar un Chromium de mas.
            if (browser is { IsClosed: false })
            {
                return browser;
            }

            var launchOptions = new LaunchOptions
            {
                Headless = true,
                Args =
                [
                    "--no-sandbox",
                    "--disable-setuid-sandbox",
                    // SIN ESTO CHROMIUM REVIENTA EN KUBERNETES. El /dev/shm por defecto de un
                    // contenedor son 64 MiB, y el navegador lo usa como memoria compartida: al
                    // llenarse, las pestañas mueren con "Target closed" sin explicacion. Con esta
                    // bandera usa memoria normal del proceso, que es la que si esta acotada por el
                    // limite del contenedor.
                    "--disable-dev-shm-usage",
                    "--disable-gpu"
                ]
            };

            var executablePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");

            if (!string.IsNullOrEmpty(executablePath))
            {
                launchOptions.ExecutablePath = executablePath;
            }
            else
            {
                // Descargar el navegador en caliente es un ultimo recurso: son ~150 MB dentro del
                // contenedor. En Staging no ocurre porque el Dockerfile instala chromium y define
                // PUPPETEER_EXECUTABLE_PATH.
                logger.LogWarning("PUPPETEER_EXECUTABLE_PATH is not set. Downloading a browser at runtime.");

                await new BrowserFetcher().DownloadAsync();
            }

            browser = await Puppeteer.LaunchAsync(launchOptions);

            BrowserLaunches.Add(1);

            logger.LogInformation("Chromium launched and shared for this process.");

            return browser;
        }
        finally
        {
            browserLock.Release();
        }
    }

    /// <summary>
    /// Cierra el navegador compartido al detenerse el proceso.
    /// </summary>
    /// <returns>Tarea que representa la operación de cierre.</returns>
    public async ValueTask DisposeAsync()
    {
        var instance = browser;

        if (instance is not null)
        {
            try
            {
                await instance.CloseAsync();
                await instance.DisposeAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error closing the shared browser during shutdown.");
            }
        }

        renderSlots.Dispose();
        browserLock.Dispose();
    }
}
