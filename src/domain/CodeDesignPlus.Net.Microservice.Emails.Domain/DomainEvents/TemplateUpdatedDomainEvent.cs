namespace CodeDesignPlus.Net.Microservice.Emails.Domain.DomainEvents;

[EventKey<TemplateAggregate>(1, "TemplateUpdatedDomainEvent", autoCreate: false)]
public class TemplateUpdatedDomainEvent(
     Guid aggregateId,
     Guid? eventId = null,
     Instant? occurredAt = null,
     Dictionary<string, object>? metadata = null
) : TemplateBaseDomainEvent(aggregateId, eventId, occurredAt, metadata)
{
     public static TemplateUpdatedDomainEvent Create(Guid aggregateId, string name, string subject, string body, List<string> variables, List<string> attachments, Guid? tenant)
    {
        return new TemplateUpdatedDomainEvent(aggregateId)
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
