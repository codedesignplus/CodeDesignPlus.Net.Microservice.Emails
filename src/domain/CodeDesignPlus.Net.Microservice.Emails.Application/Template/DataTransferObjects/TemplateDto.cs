namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;

public class TemplateDto : IDtoBase
{
    public required Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string From { get; set; } = null!;
    public string Alias { get; set; } = null!;
    public bool IsHtml { get; set; }
    public List<string> Variables { get; set; } = [];
    public List<FileAttachmentDto> Attachments { get; set; } = [];
    public Guid? Tenant { get; set; } = null!;
}