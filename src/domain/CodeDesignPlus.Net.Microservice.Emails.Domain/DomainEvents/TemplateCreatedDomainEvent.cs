namespace CodeDesignPlus.Net.Microservice.Emails.Domain.DomainEvents;

[EventKey<TemplateAggregate>(1, "TemplateCreatedDomainEvent")]
public class TemplateCreatedDomainEvent(
     Guid aggregateId,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : TemplateBaseDomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public static TemplateCreatedDomainEvent Create(Guid aggregateId, string name, string subject, string body, List<string> variables, List<string> attachments, Guid? tenant)
    {
        return new TemplateCreatedDomainEvent(aggregateId)
        {
            Name = name,
            Subject = subject,
            Body = body,
            Variables = variables,
            Attachments = attachments,
            Tenant = tenant
        };
    }
}
