using CodeDesignPlus.Net.Microservice.Emails.Domain.Models;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Commands.SendEmail;

/// <summary>
/// Sends an email with a template and stores it in the history.
/// </summary>
/// <param name="Tenant">
/// The tenant that sends it. The worker has no tenant in its user context, so it passes the one of the event; without
/// it the email is stored with the tenant of the user context (pendings/680).
/// </param>
public record SendEmailCommand(Guid Id, Guid IdTemplate, List<string> To, List<string> Cc, List<string> Bcc, string Subject, List<Attachment> Attachments, Dictionary<string, string> Values, Guid? Tenant = null) : IRequest;

public class Validator : AbstractValidator<SendEmailCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.To).NotEmpty().NotNull();
        RuleFor(x => x.Subject).NotEmpty().NotNull();
        RuleFor(x => x.Values).NotEmpty().NotNull();
    }   
}
