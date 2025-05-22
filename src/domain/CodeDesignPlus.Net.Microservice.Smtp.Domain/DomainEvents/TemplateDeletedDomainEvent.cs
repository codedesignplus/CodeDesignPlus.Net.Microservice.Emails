namespace CodeDesignPlus.Net.Microservice.Smtp.Domain.DomainEvents;

[EventKey<TemplateAggregate>(1, "TemplateDeletedDomainEvent")]
public class TemplateDeletedDomainEvent(
     Guid aggregateId,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : TemplateBaseDomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public static TemplateDeletedDomainEvent Create(Guid aggregateId, string name, string subject, string body, List<string> variables, List<string> attachments, Guid? tenant)
    {
        return new TemplateDeletedDomainEvent(aggregateId)
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
