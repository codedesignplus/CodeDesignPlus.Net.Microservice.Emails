namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails.DataTransferObjects;

/// <summary>
/// El detalle de un correo enviado, tal como lo devuelve el API y lo guarda el cache.
/// </summary>
/// <remarks>
/// Todo con <c>set</c> publico: el cache lo guarda con Newtonsoft, que solo asigna lo que tiene <c>set</c> publico, y
/// con <c>set</c> privado el detalle volvia vacio a partir de la segunda lectura (pendings/381).
/// </remarks>
public class EmailsDto : IDtoBase
{
    public required Guid Id { get; set; }    
    public List<string> To { get; set; } = [];
    public List<string> Cc { get; set; } = [];
    public List<string> Bcc { get; set; } = [];
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string From { get; set; } = null!;
    /// <summary>
    /// Los nombres de los adjuntos; el contenido no viaja en el detalle.
    /// </summary>
    public List<string> Attachments { get; set; } = [];
    public bool IsHtml { get; set; }
    public Dictionary<string, string> Values { get; set; } = [];
    public string? Code { get; set; } = null!;
    public string? Error { get; set; } = null!;
    public Guid? Tenant { get; set; } = null!;
}