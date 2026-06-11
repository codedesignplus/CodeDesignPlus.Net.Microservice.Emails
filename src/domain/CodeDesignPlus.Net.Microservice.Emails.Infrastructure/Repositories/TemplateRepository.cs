using CodeDesignPlus.Net.Microservice.Emails.Domain;
using MongoDB.Driver;

namespace CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Repositories;

public class TemplateRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<TemplateRepository> logger)
    : RepositoryBase(serviceProvider, mongoOptions, logger), ITemplateRepository
{
    public async Task<TemplateAggregate> FindByNameAndTenantAsync(string name, Guid? tenant, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection<TemplateAggregate>();

        var filter = Builders<TemplateAggregate>.Filter.And(
            Builders<TemplateAggregate>.Filter.Eq(x => x.Name, name),
            Builders<TemplateAggregate>.Filter.Eq(x => x.Tenant, tenant),
            Builders<TemplateAggregate>.Filter.Eq(x => x.IsActive, true)
        );

        return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAndTenantAsync(string name, Guid? tenant, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection<TemplateAggregate>();

        var filter = Builders<TemplateAggregate>.Filter.And(
            Builders<TemplateAggregate>.Filter.Eq(x => x.Name, name),
            Builders<TemplateAggregate>.Filter.Eq(x => x.Tenant, tenant)
        );

        return await collection.Find(filter).AnyAsync(cancellationToken);
    }

    public async Task<List<TemplateAggregate>> GetSystemTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var collection = GetCollection<TemplateAggregate>();

        var filter = Builders<TemplateAggregate>.Filter.And(
            Builders<TemplateAggregate>.Filter.Eq(x => x.Tenant, (Guid?)null),
            Builders<TemplateAggregate>.Filter.Eq(x => x.IsActive, true)
        );

        return await collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<List<TemplateAggregate>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection<TemplateAggregate>();

        var filter = Builders<TemplateAggregate>.Filter.And(
            Builders<TemplateAggregate>.Filter.Eq(x => x.Tenant, (Guid?)tenantId),
            Builders<TemplateAggregate>.Filter.Eq(x => x.IsActive, true)
        );

        return await collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var collection = GetCollection<TemplateAggregate>();

        var filter = Builders<TemplateAggregate>.Filter.And(
            Builders<TemplateAggregate>.Filter.Eq(x => x.Tenant, (Guid?)tenantId),
            Builders<TemplateAggregate>.Filter.Eq(x => x.IsActive, true)
        );

        return await collection.Find(filter).AnyAsync(cancellationToken);
    }
}