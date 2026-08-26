using System.ComponentModel.DataAnnotations;

namespace CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Options;

/// <summary>
/// Configuracion del generador de PDF.
/// </summary>
/// <remarks>
/// Los dos valores existen por el mismo motivo: un Chromium no cabe donde cabe un microservicio.
/// El 26-ago-2026 el pod murio con OOMKill (exitCode 137) generando un recibo, con un limite de
/// 320Mi y un consumo en reposo de 110-112 MiB. Renderizar un A4 no entra en los ~208 MiB que
/// quedaban.
/// </remarks>
public class PdfGeneratorOptions
{
    /// <summary>
    /// Nombre de la seccion en el appsettings.
    /// </summary>
    public const string Section = "PdfGenerator";

    /// <summary>
    /// Numero maximo de renderizados simultaneos.
    /// </summary>
    /// <remarks>
    /// Es el techo de memoria del proceso, no una preferencia. Cada renderizado abre una pestaña
    /// y cada pestaña cuesta memoria: sin este limite, dos recibos a la vez vuelven a matar el pod
    /// por muy alto que se ponga el limite del contenedor. Las peticiones que no caben ESPERAN,
    /// no fallan.
    /// </remarks>
    [Range(1, 32)]
    public int MaxConcurrentRenders { get; set; } = 2;

    /// <summary>
    /// Tiempo maximo que una peticion espera su turno antes de rendirse, en segundos.
    /// </summary>
    /// <remarks>
    /// Sin este tope, una avalancha convierte la cola de espera en una fuga de memoria: las
    /// peticiones se acumulan esperando un turno que no llega. Es preferible fallar rapido y que
    /// el llamante reintente.
    /// </remarks>
    [Range(1, 600)]
    public int QueueTimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Tiempo maximo para renderizar un documento, en segundos.
    /// </summary>
    /// <remarks>
    /// Una plantilla que referencie un recurso externo inalcanzable deja el Networkidle0 colgado.
    /// Sin tope, esa pestaña ocupa su hueco de concurrencia para siempre.
    /// </remarks>
    [Range(1, 600)]
    public int RenderTimeoutSeconds { get; set; } = 60;
}
