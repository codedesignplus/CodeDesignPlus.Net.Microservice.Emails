using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.SendMailPasswordTemp;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Consumers;

[QueueName("User", "sendmailpasswordtemp")]
public class SendMailPasswordTempHandler(IMediator mediator, ILogger<SendMailPasswordTempHandler> logger) : IEventHandler<UserCreatedDomainEvent>
{
    public async Task HandleAsync(UserCreatedDomainEvent data, CancellationToken token)
    {
        if(data.WasCreatedFromSSO)
            return;

        try
        {
            var command = new SendMailPasswordTempCommand(
                data.AggregateId,
                data.FirstName,
                data.LastName,
                data.DisplayName,
                data.Email,
                data.Phone,
                data.PasswordKey!,
                data.PasswordCipher!,
                data.IsActive
            );

            await mediator.Send(command, token);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send temp password email to {Email} for user {AggregateId}. Skipping.", data.Email, data.AggregateId);
        }
    }
}
