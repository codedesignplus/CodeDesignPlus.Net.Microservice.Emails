using CodeDesignPlus.Net.Microservice.Emails.Domain.Enums;

namespace CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Repositories;

public class UserRepository(IServiceProvider serviceProvider, IOptions<MongoOptions> mongoOptions, ILogger<UserRepository> logger)

    : RepositoryBase(serviceProvider, mongoOptions, logger), IUserRepository
{
    public Task<UserAggregate> GetByTemplateAsync(TypeTemplate typeTemplate, CancellationToken cancellationToken = default)
    {
        var collection = this.GetCollection<UserAggregate>();

        return collection.Find(x => x.Type == typeTemplate).FirstOrDefaultAsync(cancellationToken);
    }

}