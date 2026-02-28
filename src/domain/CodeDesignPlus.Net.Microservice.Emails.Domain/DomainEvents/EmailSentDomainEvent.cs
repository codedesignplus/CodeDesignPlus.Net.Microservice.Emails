namespace CodeDesignPlus.Net.Microservice.Emails.Domain.DomainEvents;

[EventKey<EmailsAggregate>(1, "EmailSentDomainEvent")]
public class EmailSentDomainEvent(
     Guid aggregateId,
     List<string> to,
     List<string> cc,
     List<string> bcc,
     string subject,
     string body,
     string from,
     List<string> attachments,
     bool isHtml,
     Dictionary<string, string> values,
     string? code,
     string? error,
     Guid? tenant,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public List<string> To { get; private set; } = to;
    public List<string> Cc { get; private set; } = cc;
    public List<string> Bcc { get; private set; } = bcc;
    public string Subject { get; private set; } = subject;
    public string Body { get; private set; } = body;
    public string From { get; private set; } = from;
    public List<string> Attachments { get; private set; } = attachments;
    public bool IsHtml { get; private set; } = isHtml;
    public Dictionary<string, string> Values { get; private set; } = values;
    public string? Code { get; private set; } = code;
    public string? Error { get; private set; } = error;
    public Guid? Tenant { get; private set; } = tenant;

    public static EmailSentDomainEvent Create(Guid aggregateId, List<string> to, List<string> cc, List<string> bcc, string subject, string body, string from, List<string> attachments, bool isHtml, Dictionary<string, string> values, string? code, string? error, Guid? tenant)
    {
        return new EmailSentDomainEvent(aggregateId, to, cc, bcc, subject, body, from, attachments, isHtml, values, code, error, tenant);
    }
}
