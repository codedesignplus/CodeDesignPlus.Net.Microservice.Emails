using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.GeneratePdf;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.RenderTemplate;
using Google.Protobuf;

namespace CodeDesignPlus.Net.Microservice.Emails.gRpc.Services;

public class EmailsService(IMediator mediator) : Emails.EmailsBase
{
    public override async Task<RenderTemplateResponse> RenderTemplate(RenderTemplateRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new RenderTemplateQuery(request.TemplateType, request.Values.ToDictionary(x => x.Key, x => x.Value)),
            context.CancellationToken
        );

        return new RenderTemplateResponse
        {
            RenderedHtml = result.RenderedHtml,
            Subject = result.Subject,
            Success = result.Success,
            Error = result.Error ?? string.Empty
        };
    }

    public override async Task<GeneratePdfResponse> GeneratePdf(GeneratePdfRequest request, ServerCallContext context)
    {
        var result = await mediator.Send(
            new GeneratePdfCommand(request.TemplateType, request.Values.ToDictionary(x => x.Key, x => x.Value)),
            context.CancellationToken
        );

        return new GeneratePdfResponse
        {
            PdfContent = ByteString.CopyFrom(result.PdfContent),
            Success = result.Success,
            Error = result.Error ?? string.Empty
        };
    }
}
