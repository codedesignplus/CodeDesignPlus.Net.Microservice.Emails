using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.SendMailPasswordTemp;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Consumers;

[QueueName("User", "sendmailpasswordtemp")]
public class SendMailPasswordTempHandler(IMediator mediator) : IEventHandler<UserCreatedDomainEvent>
{
    public Task HandleAsync(UserCreatedDomainEvent data, CancellationToken token)
    {
        if(data.WasCreatedFromSSO)
            return Task.CompletedTask;
        
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

        return mediator.Send(command, token);
    }
}
