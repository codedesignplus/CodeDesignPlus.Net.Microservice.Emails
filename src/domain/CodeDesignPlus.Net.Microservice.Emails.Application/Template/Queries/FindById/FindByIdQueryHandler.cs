using CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.FindById;

/// <summary>
/// Reads a template of the session's tenant or a global one, from the cache when it is there (pendings/674).
/// </summary>
/// <remarks>
/// Another tenant's copy answers like one that does not exist, as <c>GetEmailsById</c> since pendings/680: the copy carries
/// the tenant's own texts and attachments. The cache is looked up only under the session's tenant and the global owner.
/// </remarks>
public class FindByIdQueryHandler(ITemplateRepository repository, IMapper mapper, ICacheManager cacheManager, IUserContext user) : IRequestHandler<FindByIdQuery, TemplateDto>
{
    public async Task<TemplateDto> Handle(FindByIdQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        foreach (var key in new[] { TemplateCacheKey.For(user.Tenant, request.Id), TemplateCacheKey.For(null, request.Id) }.Distinct())
        {
            if (await cacheManager.ExistsAsync(key))
            {
                return await cacheManager.GetAsync<TemplateDto>(key);
            }
        }

        var template = await repository.FindAsync<TemplateAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsFalse(template?.IsVisibleTo(user.Tenant) == true, Errors.TemplateNotFound);

        var dto = mapper.Map<TemplateDto>(template);

        await cacheManager.SetAsync(TemplateCacheKey.For(template!.Tenant, template.Id), dto);

        return dto;
    }
}
