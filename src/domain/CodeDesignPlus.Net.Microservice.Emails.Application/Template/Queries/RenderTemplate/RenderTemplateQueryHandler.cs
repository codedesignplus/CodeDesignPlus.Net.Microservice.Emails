using CodeDesignPlus.Net.Microservice.Emails.Domain.Enums;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.RenderTemplate;

public class RenderTemplateQueryHandler(ITemplateRepository templateRepository, IEmailSender emailSender, IUserContext userContext) : IRequestHandler<RenderTemplateQuery, RenderTemplateResult>
{
    public async Task<RenderTemplateResult> Handle(RenderTemplateQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        if (!Enum.TryParse<TypeTemplate>(request.TemplateType, true, out _))
            return new RenderTemplateResult(string.Empty, string.Empty, string.Empty, false, $"Invalid template type: {request.TemplateType}");

        var templateName = request.TemplateType;

        TemplateAggregate template = null;

        if (userContext.Tenant != Guid.Empty)
            template = await templateRepository.FindByNameAndTenantAsync(templateName, userContext.Tenant, cancellationToken);

        template ??= await templateRepository.FindByNameAndTenantAsync(templateName, null, cancellationToken);

        if (template == null)
            return new RenderTemplateResult(string.Empty, string.Empty, string.Empty, false, $"Template not found: {templateName}");

        var renderedHtml = emailSender.BuildBody(template.Body, request.Values);
        var renderedSubject = BuildSubject(template.Subject, request.Values);

        return new RenderTemplateResult(renderedHtml, renderedSubject, template.From, true, string.Empty);
    }

    private static string BuildSubject(string subject, Dictionary<string, string> values)
    {
        var result = subject;

        foreach (var kvp in values)
            result = result.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);

        return result;
    }
}
