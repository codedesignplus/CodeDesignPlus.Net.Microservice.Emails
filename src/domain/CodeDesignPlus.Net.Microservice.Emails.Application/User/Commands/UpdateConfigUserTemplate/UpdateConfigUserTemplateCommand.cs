using CodeDesignPlus.Net.Microservice.Emails.Domain.Enums;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.UpdateConfigUserTemplate;

[DtoGenerator]
public record UpdateConfigUserTemplateCommand(Guid Id, Guid IdTemplate, TypeTemplate TypeTemplate, string Subject, string UriLoginApp) : IRequest;

public class Validator : AbstractValidator<UpdateConfigUserTemplateCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.IdTemplate).NotEmpty().NotNull();
        RuleFor(x => x.TypeTemplate).NotEqual(TypeTemplate.None);
        RuleFor(x => x.Subject).NotEmpty().NotNull();
        RuleFor(x => x.UriLoginApp).NotEmpty().NotNull();
    }
}
