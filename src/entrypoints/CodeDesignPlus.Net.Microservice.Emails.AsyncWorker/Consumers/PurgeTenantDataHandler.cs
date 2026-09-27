using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Consumers;

/// <summary>
/// Al purgarse una copropiedad, borra sus plantillas y los correos que envió.
/// </summary>
/// <remarks>
/// Lo publica ms-tenants cuando vence el plazo para restaurar una copropiedad eliminada, y puede llegar más de una
/// vez: borrar lo que ya no está es inofensivo.
/// <para>
/// <b>Las plantillas de sistema no se tocan.</b> Tienen <c>Tenant = null</c> y el borrado filtra
/// <c>Tenant == id</c> de la copropiedad, así que nunca las alcanza. Por eso aquí solo se pasa el id del evento, nunca
/// un valor vacío.
/// </para>
/// <para>
/// <c>PurgeTenantDataHandlerTest</c> recorre el dominio y exige que se purgue todo tipo con <c>Tenant</c>: un
/// agregado nuevo que no se añada aquí hace fallar la prueba (regla 47).
/// </para>
/// </remarks>
[QueueName<TemplateAggregate>("PurgeTenantDataHandler")]
public class PurgeTenantDataHandler(ITemplateRepository repository, ILogger<PurgeTenantDataHandler> logger) : IEventHandler<TenantPurgedDomainEvent>
{
    public async Task HandleAsync(TenantPurgedDomainEvent data, CancellationToken token)
    {
        var templates = await repository.DeleteByTenantAsync<TemplateAggregate>(data.AggregateId, token);
        var emails = await repository.DeleteByTenantAsync<EmailsAggregate>(data.AggregateId, token);

        logger.LogInformation("Tenant {TenantId} purged: {Templates} templates and {Emails} emails deleted", data.AggregateId, templates, emails);
    }
}
