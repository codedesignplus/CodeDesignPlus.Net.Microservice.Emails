namespace CodeDesignPlus.Net.Microservice.Emails.Domain.Repositories;

public interface ITemplateRepository : IRepositoryBase
{
    Task<TemplateAggregate> FindByNameAndTenantAsync(string name, Guid? tenant, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAndTenantAsync(string name, Guid? tenant, CancellationToken cancellationToken = default);
    Task<List<TemplateAggregate>> GetSystemTemplatesAsync(CancellationToken cancellationToken = default);
    Task<List<TemplateAggregate>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Guarda el registro nuevo si su id no existe todavía (pendings/589, regla 66).
    /// </summary>
    /// <param name="template">El agregado nuevo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns><c>false</c> si otro envío del mismo formulario ya lo guardó: el segundo choca contra la clave primaria.</returns>
    Task<bool> CreateIfAbsentAsync(TemplateAggregate template, CancellationToken cancellationToken);
}