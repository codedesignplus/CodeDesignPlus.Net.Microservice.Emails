namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.DeleteTemplate;

[DtoGenerator]
public record DeleteTemplateCommand(Guid Id) : IRequest;

public class Validator : AbstractValidator<DeleteTemplateCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
    }
}
