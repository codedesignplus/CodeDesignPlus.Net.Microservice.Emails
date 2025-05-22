namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Commands.UpdateTemplate;

[DtoGenerator]
public record UpdateTemplateCommand(Guid Id, string Name, string Subject, string Body, List<string> Variables, List<string> Attachments, Guid? Tenant) : IRequest;

public class Validator : AbstractValidator<UpdateTemplateCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Name).NotEmpty().NotNull();
        RuleFor(x => x.Subject).NotEmpty().NotNull();
        RuleFor(x => x.Body).NotEmpty().NotNull();
        RuleFor(x => x.Tenant).NotEmpty().NotNull();
    }
}
