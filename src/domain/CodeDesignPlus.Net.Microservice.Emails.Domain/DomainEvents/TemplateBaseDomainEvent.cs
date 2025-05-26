namespace CodeDesignPlus.Net.Microservice.Emails.Domain.DomainEvents;

public abstract class TemplateBaseDomainEvent(
    Guid aggregateId,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; protected set; } = null!;
    public string Subject { get; protected set; } = null!;
    public string Body { get; protected set; } = null!;
    public List<string> Variables { get; protected set; } = [];
    public List<string> Attachments { get; protected set; } = [];
    public Guid? Tenant { get; protected set; }
}
