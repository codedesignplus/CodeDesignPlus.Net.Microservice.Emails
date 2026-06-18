namespace CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.SendMailWelcomeToTenant;

[DtoGenerator]
public record SendMailWelcomeToTenantCommand(Guid Id, string Email, string? DisplayName, Guid TenantId, string TenantName) : IRequest;

public class Validator : AbstractValidator<SendMailWelcomeToTenantCommand>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull();
        RuleFor(x => x.Email).NotEmpty().NotNull().EmailAddress();
        RuleFor(x => x.TenantId).NotEmpty().NotNull();
        RuleFor(x => x.TenantName).NotEmpty().NotNull();
    }
}
