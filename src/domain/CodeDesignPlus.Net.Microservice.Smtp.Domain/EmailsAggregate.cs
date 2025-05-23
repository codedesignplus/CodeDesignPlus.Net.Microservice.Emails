namespace CodeDesignPlus.Net.Microservice.Smtp.Domain;

public class EmailsAggregate(Guid id) : AggregateRootBase(id)
{
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

    public EmailsAggregate(Guid id, List<string> to, List<string> cc, List<string> bcc, string subject, string body, string from, List<string> attachments, bool isHtml, Dictionary<string, string> values, string? code, string? error, Guid? tenant) : this(id)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdEmailIsInvalid);
        DomainGuard.IsEmpty(to, Errors.ToEmailIsInvalid);
        DomainGuard.IsNullOrEmpty(subject, Errors.SubjectEmailIsInvalid);
        DomainGuard.IsNullOrEmpty(body, Errors.BodyEmailIsInvalid);
        DomainGuard.IsEmpty(from, Errors.FromEmailIsInvalid);

        To = to;
        Cc = cc;
        Bcc = bcc;
        Subject = subject;
        Body = body;
        From = from;
        Attachments = attachments;
        IsHtml = isHtml;
        Values = values;
        Code = code;
        Error = error;
        Tenant = tenant;

        CreatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(EmailSentDomainEvent.Create(Id, To, Cc, Bcc, Subject, Body, From, Attachments, IsHtml, Values, Code, Error, Tenant));
    }

    public static EmailsAggregate Create(Guid id, List<string> to, List<string> cc, List<string> bcc, string subject, string body, string from, List<string> attachments, bool isHtml, Dictionary<string, string> values, string? code, string? error, Guid? tenant)
    {
        return new EmailsAggregate(id, to, cc, bcc, subject, body, from, attachments, isHtml, values, code, error, tenant);
    }
}
