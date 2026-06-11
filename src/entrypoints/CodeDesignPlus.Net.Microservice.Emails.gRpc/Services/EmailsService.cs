using CodeDesignPlus.Net.File.Storage.Abstractions;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.GeneratePdf;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.RenderTemplate;

namespace CodeDesignPlus.Net.Microservice.Emails.gRpc.Services;

public class EmailsService(IMediator mediator, IFileStorage fileStorage) : Emails.EmailsBase
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
        var tenant = Guid.Parse(request.Tenant);

        var result = await mediator.Send(
            new GeneratePdfCommand(request.TemplateType, request.Values.ToDictionary(x => x.Key, x => x.Value)),
            context.CancellationToken
        );

        if (!result.Success)
            return new GeneratePdfResponse { Success = false, Error = result.Error ?? string.Empty };

        var fileId = Guid.NewGuid();
        var fileName = $"{request.TemplateType}-{fileId}.pdf";
        var target = $"emails-pdf/{tenant}";

        using var stream = new MemoryStream(result.PdfContent);
        await fileStorage.UploadAsync(stream, fileName, target, false, tenant, context.CancellationToken);

        var signedResponse = await fileStorage.GetSignedUrlAsync(fileName, target, TimeSpan.FromDays(7), tenant, context.CancellationToken);

        return new GeneratePdfResponse
        {
            Id = fileId.ToString(),
            Name = fileName,
            Target = target,
            SignedUrl = signedResponse?.Success == true ? signedResponse.File.Detail.SignedUrl.ToString() : string.Empty,
            Success = true,
            Error = string.Empty
        };
    }
}
