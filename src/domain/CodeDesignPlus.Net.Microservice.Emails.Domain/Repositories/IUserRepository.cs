using CodeDesignPlus.Net.Microservice.Emails.Domain.Enums;

namespace CodeDesignPlus.Net.Microservice.Emails.Domain.Repositories;

public interface IUserRepository : IRepositoryBase
{
    Task<UserAggregate> GetByTemplateAsync(TypeTemplate typeTemplate, CancellationToken cancellationToken = default);
}