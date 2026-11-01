using CodeDesignPlus.Net.Microservice.Emails.Domain.Models;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Services;
using Microsoft.AspNetCore.Http;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Commands.SendEmail;

public class SendEmailCommandHandler(IEmailsRepository repository, IPubSub pubsub, IUserContext user, IEmailSender emailSender) : IRequestHandler<SendEmailCommand>
{
    /// <summary>
    /// What the history keeps in place of the value of a sensitive variable.
    /// </summary>
    public const string MaskedValue = "••••••";

    public async Task Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exist = await repository.ExistsAsync<EmailsAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.EmailsAlreadyWasSent);

        var existTemplate = await repository.ExistsAsync<TemplateAggregate>(request.IdTemplate, cancellationToken);

        ApplicationGuard.IsFalse(existTemplate, Errors.TemplateNotFound);

        var template = await repository.FindAsync<TemplateAggregate>(request.IdTemplate, cancellationToken);

        // El asunto viaja en texto plano y el cuerpo en base64: SendEmail decodifica solo el cuerpo.
        // Pasar el asunto por BuildBody lo trataba como base64 y reventaba con cualquier plantilla.
        var subject = emailSender.BuildSubject(template.Subject, request.Values);
        var body = emailSender.BuildBody(template.Body, request.Values);
        var from = template.From;
        var alias = template.Alias;
        var isHtml = template.IsHtml;
        var attachments = request.Attachments;

        var message = EmailMessage.Create(subject, body, request.To, request.Cc, request.Bcc, from, alias, attachments, isHtml);

        var response = await emailSender.SendEmail(message, cancellationToken);

        // El correo sale con los valores reales; el historial y el evento los llevan enmascarados, para que una
        // contraseña temporal o un código no queden guardados ni se lean en el detalle (pendings/680).
        var masks = template.SensitiveVariables.Any(request.Values.ContainsKey);
        var storedValues = masks ? Mask(request.Values, template.SensitiveVariables) : request.Values;
        var storedSubject = masks ? emailSender.BuildSubject(template.Subject, storedValues) : subject;
        var storedBody = masks ? emailSender.BuildBody(template.Body, storedValues) : body;

        var tenant = request.Tenant ?? user.Tenant;

        var aggregate = EmailsAggregate.Create(request.Id, request.To, request.Cc, request.Bcc, storedSubject, storedBody, from, attachments, isHtml, storedValues, response.StatusCode, response.Error, tenant, response.Succeeded);

        await repository.CreateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }

    private static Dictionary<string, string> Mask(Dictionary<string, string> values, List<string> sensitiveVariables)
    {
        return values.ToDictionary(kvp => kvp.Key, kvp => sensitiveVariables.Contains(kvp.Key) ? MaskedValue : kvp.Value);
    }
}
