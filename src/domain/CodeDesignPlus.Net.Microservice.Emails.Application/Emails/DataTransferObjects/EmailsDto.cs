namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails.DataTransferObjects;

public class EmailsDto : IDtoBase
{
    public required Guid Id { get; set; }    
    public List<string> To { get; private set; } = [];
    public List<string> Cc { get; private set; } = [];
    public List<string> Bcc { get; private set; } = [];
    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string From { get; private set; } = null!;
    public List<string> Attachments { get; private set; } = [];
    public bool IsHtml { get; private set; }
    public Dictionary<string, string> Values { get; private set; } = [];
    public string? Code { get; private set; } = null!;
    public string? Error { get; private set; } = null!;
    public Guid? Tenant { get; private set; } = null!;
}