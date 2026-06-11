using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Emails.Domain;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Repositories;

namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Consumers;

[QueueName<TemplateAggregate>("ProvisionTenantHandler")]
public class ProvisionTenantHandler(
    ITemplateRepository templateRepository,
    ILogger<ProvisionTenantHandler> logger) : IEventHandler<TenantCreatedDomainEvent>
{
    private static readonly Guid SystemUserId = Guid.Parse("10000000-0000-0000-0000-000000000001");

    public async Task HandleAsync(TenantCreatedDomainEvent data, CancellationToken token)
    {
        var tenantId = data.AggregateId;

        var systemTemplates = await templateRepository.GetSystemTemplatesAsync(token);

        if (systemTemplates.Count == 0)
        {
            logger.LogWarning("No system templates found to clone for tenant {TenantId}.", tenantId);
            return;
        }

        var cloned = 0;

        foreach (var template in systemTemplates)
        {
            var exists = await templateRepository.ExistsByNameAndTenantAsync(template.Name, tenantId, token);

            if (exists)
                continue;

            var clone = TemplateAggregate.Create(
                Guid.NewGuid(),
                template.Name,
                template.Subject,
                template.Body,
                [.. template.Variables],
                [.. template.Attachments],
                template.From,
                template.Alias,
                template.IsHtml,
                tenantId,
                SystemUserId
            );

            await templateRepository.CreateAsync(clone, token);
            cloned++;
        }

        logger.LogInformation("Provisioned {Cloned} of {Total} templates for tenant {TenantId}.", cloned, systemTemplates.Count, tenantId);
    }
}
