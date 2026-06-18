using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.SendMailWelcomeToTenant;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Consumers;

/// <summary>
/// Listens for <c>TenantAddedDomainEvent</c> from ms-users and dispatches the
/// <c>SendMailWelcomeToTenantCommand</c>, which renders the InvitationToOrganization
/// template with the tenant's name. Independent of the PasswordTemp flow that fires
/// when ms-microsoftgraph provisions the Azure AD identity.
/// </summary>
[QueueName("User", "sendmailwelcometotenant")]
public class SendMailWelcomeToTenantHandler(IMediator mediator) : IEventHandler<TenantAddedDomainEvent>
{
    public Task HandleAsync(TenantAddedDomainEvent data, CancellationToken token)
    {
        var command = new SendMailWelcomeToTenantCommand(
            data.AggregateId,
            data.Email,
            data.DisplayName,
            data.Tenant.Id,
            data.Tenant.Name
        );

        return mediator.Send(command, token);
    }
}
