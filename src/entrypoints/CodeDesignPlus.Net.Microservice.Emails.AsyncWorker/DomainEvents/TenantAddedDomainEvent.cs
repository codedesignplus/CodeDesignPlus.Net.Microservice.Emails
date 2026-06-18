namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;

/// <summary>
/// External event raised by ms-users when an existing UserAggregate gets associated
/// to a tenant via <c>UserAggregate.AddTenant()</c>. We mirror its shape here so the
/// AsyncWorker can subscribe and trigger the welcome email containing the tenant name.
/// </summary>
[EventKey("UserAggregate", 1, "TenantAddedDomainEvent", "ms-users")]
public class TenantAddedDomainEvent(
    Guid aggregateId,
    string? displayName,
    string email,
    TenantInfo tenant,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string? DisplayName { get; } = displayName;
    public string Email { get; } = email;
    public TenantInfo Tenant { get; } = tenant;
}

public class TenantInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
