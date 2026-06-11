using System.Text;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.RenderTemplate;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.GeneratePdf;

public class GeneratePdfCommandHandler(
    IMediator mediator,
    IPdfGenerator pdfGenerator
) : IRequestHandler<GeneratePdfCommand, GeneratePdfResult>
{
    public async Task<GeneratePdfResult> Handle(GeneratePdfCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var renderResult = await mediator.Send(new RenderTemplateQuery(request.TemplateType, request.Values), cancellationToken);

        if (!renderResult.Success)
            return new GeneratePdfResult(Guid.Empty, string.Empty, string.Empty, string.Empty, false, renderResult.Error);

        var html = Encoding.UTF8.GetString(Convert.FromBase64String(renderResult.RenderedHtml));

        var pdfBytes = await pdfGenerator.GenerateFromHtmlAsync(html, cancellationToken);

        return new GeneratePdfResult(Guid.Empty, string.Empty, string.Empty, string.Empty, true, string.Empty, pdfBytes);
    }
}
