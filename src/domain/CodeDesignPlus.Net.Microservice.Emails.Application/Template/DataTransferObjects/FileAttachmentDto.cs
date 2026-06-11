namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;

public class FileAttachmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Target { get; set; } = null!;
}
