namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;

public class TemplateDto : IDtoBase
{
    public required Guid Id { get; set; }
    public string Name { get; private set; } = null!;
    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string From { get; private set; } = null!;
    public string Alias { get; private set; } = null!;
    public bool IsHtml { get; private set; }
    public List<string> Variables { get; private set; } = [];
    public List<string> Attachments { get; private set; } = [];
    public Guid? Tenant { get; private set; } = null!;

}