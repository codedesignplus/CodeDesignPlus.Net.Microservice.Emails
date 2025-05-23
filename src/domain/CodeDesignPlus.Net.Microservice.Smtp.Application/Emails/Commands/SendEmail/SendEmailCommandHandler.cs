using CodeDesignPlus.Net.Microservice.Smtp.Domain.Models;
using CodeDesignPlus.Net.Microservice.Smtp.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Emails.Commands.SendEmail;

public class SendEmailCommandHandler(IEmailsRepository repository, IPubSub pubsub, IEmailSender emailSender) : IRequestHandler<SendEmailCommand>
{
    public async Task Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync<EmailsAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.EmailsAlreadyWasSent);

        var existTemplate = await repository.ExistsAsync<TemplateAggregate>(request.IdTemplate, cancellationToken);

        ApplicationGuard.IsFalse(existTemplate, Errors.TemplateNotFound);

        var template = await repository.FindAsync<TemplateAggregate>(request.IdTemplate, cancellationToken);

        var body = emailSender.BuildBody(template.Body, request.Values);
        var from = template.From;
        var alias = template.Alias;
        var isHtml = template.IsHtml;

        var message = EmailMessage.Create(request.Subject, body, request.To, request.Cc, request.Bcc, from, alias, request.Attachments, isHtml);

        var response = await emailSender.SendEmail(message, cancellationToken);

        var aggregate = EmailsAggregate.Create(request.Id, request.To, request.Cc, request.Bcc, request.Subject, body, from, request.Attachments, isHtml, request.Values, response.StatusCode, response.Error);

        await repository.CreateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}