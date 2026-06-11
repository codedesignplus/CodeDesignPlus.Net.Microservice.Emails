using CodeDesignPlus.Net.Microservice.Emails.Domain.Services;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Services.PdfGenerator;

public class PdfGenerator : IPdfGenerator
{
    public async Task<byte[]> GenerateFromHtmlAsync(string html, CancellationToken cancellationToken)
    {
        var launchOptions = new LaunchOptions
        {
            Headless = true,
            Args = ["--no-sandbox", "--disable-setuid-sandbox"]
        };

        var executablePath = Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH");

        if (!string.IsNullOrEmpty(executablePath))
            launchOptions.ExecutablePath = executablePath;
        else
            await new BrowserFetcher().DownloadAsync();

        await using var browser = await Puppeteer.LaunchAsync(launchOptions);
        await using var page = await browser.NewPageAsync();

        await page.SetContentAsync(html, new NavigationOptions { WaitUntil = [WaitUntilNavigation.Networkidle0] });

        var pdfBytes = await page.PdfDataAsync(new PdfOptions
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

        return pdfBytes;
    }
}
