using System;

namespace CodeDesignPlus.Net.Microservice.Smtp.Rest.DataTransferObjects;

public class SendEmailDto
{
    public Guid Id { get; set; }
    public Guid IdTemplate { get; set; }
    public List<string> To { get; set; } = [];
    public List<string> Cc { get; set; } = [];
    public List<string> Bcc { get; set; } = [];
    public string Subject { get; set; } = null!;
    public List<IFormFile> Attachments { get; set; } = [];
    public Dictionary<string, string> Values { get; set; } = [];
}