namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Emails.Commands.SendEmail;

public class SendEmailCommandHandler(IEmailsRepository repository, IPubSub pubsub) : IRequestHandler<SendEmailCommand>
{
    public async Task Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync<EmailsAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.EmailsAlreadyWasSent);

        var body = string.Empty;
        var from = string.Empty;
        var isHtml = false;

        var aggregate = EmailsAggregate.Create(request.Id, request.To, request.Cc, request.Bcc, request.Subject, body, from, request.Attachments, isHtml, request.Values, request.Status, request.Server);

        await repository.CreateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}