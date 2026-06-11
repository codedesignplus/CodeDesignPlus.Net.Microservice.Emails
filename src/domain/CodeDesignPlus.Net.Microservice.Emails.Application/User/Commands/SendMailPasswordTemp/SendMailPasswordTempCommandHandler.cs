using CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Commands.SendEmail;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Enums;
using CodeDesignPlus.Net.Vault.Abstractions;
using CodeDesignPlus.Net.Vault.Abstractions.Options;
using Microsoft.Extensions.Options;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.SendMailPasswordTemp;

public class SendMailPasswordTempCommandHandler(ITemplateRepository templateRepository, IMediator mediator, IVaultTransit vaultTransit, IOptions<VaultOptions> options) : IRequestHandler<SendMailPasswordTempCommand>
{
    private const string KEY_SECRET_CONTEXT = "vault_transit_password_temp";

    public async Task Handle(SendMailPasswordTempCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var template = await templateRepository.FindByNameAndTenantAsync(nameof(TypeTemplate.PasswordTemp), null, cancellationToken);

        ApplicationGuard.IsNull(template, Errors.TemplatePasswordTempNotFound);

        var isValidContext = options.Value.Transit.SecretContexts.TryGetValue(KEY_SECRET_CONTEXT, out var secretContext);

        DomainGuard.IsFalse(isValidContext, Errors.SecretContextNotFound);

        var password = await vaultTransit.DecryptAsync(request.PasswordKey, request.PasswordCipher, secretContext);

        var sendMailCommand = new SendEmailCommand(
            request.Id,
            template.Id,
            [request.Email],
            [],
            [],
            template.Subject,
            [],
            new Dictionary<string, string>
            {
                { "display_name", request.DisplayName ?? $"{request.FirstName} {request.LastName}" },
                { "password", password },
                { "login_app", "" },
                { "current_year", SystemClock.Instance.GetCurrentInstant().InUtc().Year.ToString() },
            });

        await mediator.Send(sendMailCommand, cancellationToken);
    }
}