using CodeDesignPlus.Net.Microservice.Smtp.Application.Emails.Commands.SendEmail;

namespace CodeDesignPlus.Net.Microservice.Smtp.gRpc.Services;

public class EmailsService(IMediator mediator) : Emails.EmailsBase
{
    public override Task<SendEmailResponse> SendEmail(SendEmailRequest request, ServerCallContext context)
    {
        if(Guid.TryParse(request.Id, out var id) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Id"));

        if(Guid.TryParse(request.IdTemplate, out var idTemplate) == false)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid IdTemplate"));

        var command = new SendEmailCommand(
            id,
            idTemplate,
            [.. request.To],
            [.. request.Cc],
            [.. request.Bcc],
            request.Subject,
            ConvertToAttachments([.. request.Attachments]),
            request.Values.ToDictionary(x => x.Key, x => x.Value)
        );

        var result = mediator.Send(command, context.CancellationToken);

        return Task.FromResult(new SendEmailResponse());
    }

    private static List<Domain.Models.Attachment> ConvertToAttachments(List<gRpc.Attachment> attachments)
    {
        var result = new List<Domain.Models.Attachment>();

        foreach (var attachment in attachments)
        {

            result.Add(Domain.Models.Attachment.Create(attachment.FileName, attachment.ContentType, attachment.Content.ToByteArray()));
        }

        return result;
    }
}