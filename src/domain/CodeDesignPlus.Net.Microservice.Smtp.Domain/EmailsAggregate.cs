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
    public string Status { get; private set; } = null!;
    public string Server { get; private set; } = null!;

    public EmailsAggregate(Guid id, List<string> to, List<string> cc, List<string> bcc, string subject, string body, string from, List<string> attachments, bool isHtml, Dictionary<string, string> values, string status, string server) : this(id)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdEmailIsInvalid);
        DomainGuard.IsEmpty(to, Errors.ToEmailIsInvalid);
        DomainGuard.IsEmpty(cc, Errors.CcEmailIsInvalid);
        DomainGuard.IsEmpty(bcc, Errors.BccEmailIsInvalid);
        DomainGuard.IsNullOrEmpty(subject, Errors.SubjectEmailIsInvalid);
        DomainGuard.IsNullOrEmpty(body, Errors.BodyEmailIsInvalid);
        DomainGuard.IsEmpty(from, Errors.FromEmailIsInvalid);
        DomainGuard.IsEmpty(attachments, Errors.AttachmentsEmailIsInvalid);
        DomainGuard.IsNullOrEmpty(server, Errors.ServerEmailIsInvalid);

        To = to;
        Cc = cc;
        Bcc = bcc;
        Subject = subject;
        Body = body;
        From = from;
        Attachments = attachments;
        IsHtml = isHtml;
        Values = values;
        Status = status;
        Server = server;

        CreatedAt = SystemClock.Instance.GetCurrentInstant();

        this.AddEvent(EmailSentDomainEvent.Create(Id, To, Cc, Bcc, Subject, Body, From, Attachments, IsHtml, Values, Status, Server));
    }

    public static EmailsAggregate Create(Guid id, List<string> to, List<string> cc, List<string> bcc, string subject, string body, string from, List<string> attachments, bool isHtml, Dictionary<string, string> values, string status, string server)
    {
        return new EmailsAggregate(id, to, cc, bcc, subject, body, from, attachments, isHtml, values, status, server);
    }
}
