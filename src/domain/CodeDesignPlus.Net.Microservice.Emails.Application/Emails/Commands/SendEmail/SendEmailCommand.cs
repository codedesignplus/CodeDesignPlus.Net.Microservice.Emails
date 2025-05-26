using CodeDesignPlus.Net.Microservice.Emails.Domain.Models;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Commands.SendEmail;

public record SendEmailCommand(Guid Id, Guid IdTemplate, List<string> To, List<string> Cc, List<string> Bcc, string Subject, List<Attachment> Attachments, Dictionary<string, string> Values) : IRequest;

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
