namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.RenderTemplate;

public record RenderTemplateQuery(string TemplateType, Dictionary<string, string> Values) : IRequest<RenderTemplateResult>;

public record RenderTemplateResult(string RenderedHtml, string Subject, string From, bool Success, string Error);
