using CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Commands.SendEmail;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Enums;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.SendMailWelcomeToTenant;

/// <summary>
/// Sends a "welcome to {tenant}" email to a user that has just been associated with a tenant.
/// Triggered by the AsyncWorker consumer of <c>TenantAddedDomainEvent</c> (ms-users).
/// Uses the <c>InvitationToOrganization</c> template — distinct from <c>PasswordTemp</c>
/// which carries the Azure AD temporary password.
/// </summary>
public class SendMailWelcomeToTenantCommandHandler(ITemplateRepository templateRepository, IMediator mediator) : IRequestHandler<SendMailWelcomeToTenantCommand>
{
    public async Task Handle(SendMailWelcomeToTenantCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        // Whoever bought the tenant was not invited to it: they get "your tenant is ready" (pendings/069).
        var (templateName, missing) = request.ByPurchase
            ? (nameof(TypeTemplate.TenantReady), Errors.TemplateTenantReadyNotFound)
            : (nameof(TypeTemplate.InvitationToOrganization), Errors.TemplateInvitationToOrganizationNotFound);

        var template = await templateRepository.FindByNameAndTenantAsync(templateName, null, cancellationToken);

        ApplicationGuard.IsNull(template, missing);

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
                { "display_name", request.DisplayName ?? request.Email },
                { "organization_name", request.TenantName },
                { "current_year", SystemClock.Instance.GetCurrentInstant().InUtc().Year.ToString() },
            });

        await mediator.Send(sendMailCommand, cancellationToken);
    }
}
