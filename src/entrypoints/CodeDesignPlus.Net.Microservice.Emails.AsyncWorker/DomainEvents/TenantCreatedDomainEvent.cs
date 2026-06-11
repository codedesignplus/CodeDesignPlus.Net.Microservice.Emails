namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;

[EventKey("TenantAggregate", 1, "TenantCreatedDomainEvent", "ms-tenants", "codedesignplus")]
public class TenantCreatedDomainEvent(
    Guid aggregateId,
    string name,
    TenantLicense license,
    TenantLocation location,
    bool isActive,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string Name { get; } = name;
    public TenantLicense License { get; } = license;
    public TenantLocation Location { get; } = location;
    public bool IsActive { get; } = isActive;
}

public class TenantLicense
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TenantModule> Modules { get; set; } = [];
}

public class TenantModule
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TenantLocation
{
    public TenantCountry Country { get; set; } = null!;
}

public class TenantCountry
{
    public string Alpha2 { get; set; } = string.Empty;
    public TenantCurrency Currency { get; set; } = null!;
}

public class TenantCurrency
{
    public string Code { get; set; } = string.Empty;
}
