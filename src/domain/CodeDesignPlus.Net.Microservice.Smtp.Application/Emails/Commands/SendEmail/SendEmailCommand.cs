namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Emails.Commands.SendEmail;

[DtoGenerator]
public record SendEmailCommand(Guid Id, List<string> To, List<string> Cc, List<string> Bcc, string Subject, List<string> Attachments, Dictionary<string, string> Values, string Status, string Server) : IRequest;

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
