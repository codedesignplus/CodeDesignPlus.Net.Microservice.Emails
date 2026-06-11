namespace CodeDesignPlus.Net.Microservice.Emails.Domain.Repositories;

public interface ITemplateRepository : IRepositoryBase
{
    Task<TemplateAggregate> FindByNameAndTenantAsync(string name, Guid? tenant, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAndTenantAsync(string name, Guid? tenant, CancellationToken cancellationToken = default);
    Task<List<TemplateAggregate>> GetSystemTemplatesAsync(CancellationToken cancellationToken = default);
    Task<List<TemplateAggregate>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}