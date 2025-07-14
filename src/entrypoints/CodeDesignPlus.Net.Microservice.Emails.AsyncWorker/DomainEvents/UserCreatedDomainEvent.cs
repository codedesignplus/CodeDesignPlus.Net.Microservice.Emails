namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;

[EventKey<UserAggregate>(1, "UserCreatedDomainEvent", "ms-microsoftgraph")]
public class UserCreatedDomainEvent(
    Guid aggregateId,
    string firstName,
    string lastName,
    string email,
    string phone,
    string? displayName,
    string? passwordKey,
    string? passwordCipher,
    bool wasCreatedFromSSO,
    bool isActive,
    Guid? eventId = null,
    Instant? occurredAt = null,
    Dictionary<string, object>? metadata = null
) : DomainEvent(aggregateId, eventId, occurredAt, metadata)
{
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public string Email { get; private set; } = email;
    public string Phone { get; private set; } = phone;
    public string? DisplayName { get; private set; } = displayName;
    public bool IsActive { get; private set; } = isActive;
    public string? PasswordKey { get; private set; } = passwordKey;
    public string? PasswordCipher { get; private set; } = passwordCipher;
    public bool WasCreatedFromSSO { get; private set; } = wasCreatedFromSSO;
    public static UserCreatedDomainEvent Create(Guid aggregateId, string firstName, string lastName, string email, string phone, string? displayName, string? passwordKey, string? passwordCipher, bool wasCreatedFromSSO, bool isActive)
    {
        return new UserCreatedDomainEvent(aggregateId, firstName, lastName, email, phone, displayName, passwordKey, passwordCipher, wasCreatedFromSSO, isActive);
    }
}