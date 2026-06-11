using CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Commands.SendEmail;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Emails.Domain;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Models;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Repositories;
using CodeDesignPlus.Net.Microservice.Emails.Domain.ValueObjects;
using CodeDesignPlus.Net.File.Storage.Abstractions;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Consumers;

[QueueName<EmailsAggregate>("SendEmailHandler")]
public class SendEmailHandler(
    IMediator mediator,
    IFileStorage fileStorage,
    ITemplateRepository templateRepository,
    ILogger<SendEmailHandler> logger
) : IEventHandler<SendEmailDomainEvent>
{
    public async Task HandleAsync(SendEmailDomainEvent data, CancellationToken token)
    {
        var template = await templateRepository.FindByNameAndTenantAsync(data.TemplateName, data.Tenant, token);
        template ??= await templateRepository.FindByNameAndTenantAsync(data.TemplateName, null, token);

        if (template == null)
        {
            logger.LogWarning("Template '{Name}' not found for tenant {Tenant} nor as system template", data.TemplateName, data.Tenant);
            return;
        }

        var attachments = await DownloadAttachmentsAsync(
            [.. data.Attachments, .. template.Attachments], data.Tenant, token);

        await mediator.Send(new SendEmailCommand(
            Guid.NewGuid(),
            template.Id,
            data.To,
            data.Cc,
            data.Bcc,
            template.Subject,
            attachments,
            data.Variables
        ), token);

        logger.LogInformation("Email sent to {To} using template '{Template}'",
            string.Join(",", data.To), data.TemplateName);
    }

    private async Task<List<Attachment>> DownloadAttachmentsAsync(
        List<FileAttachment> refs, Guid tenant, CancellationToken token)
    {
        var result = new List<Attachment>();

        foreach (var attachment in refs)
        {
            try
            {
                var response = await fileStorage.DownloadAsync(attachment.Name, attachment.Target, tenant, token);

                if (response is { Success: true, Stream: not null })
                {
                    using var ms = new MemoryStream();
                    await response.Stream.CopyToAsync(ms, token);
                    result.Add(Attachment.Create(attachment.Name, response.File.Mime.MimeType, ms.ToArray()));
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to download attachment {Name} from {Target}", attachment.Name, attachment.Target);
            }
        }

        return result;
    }
}
