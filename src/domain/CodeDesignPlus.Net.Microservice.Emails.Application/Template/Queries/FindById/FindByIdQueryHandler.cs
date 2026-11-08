using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.FindById;

public class FindByIdQueryHandler(ITemplateRepository repository, IMapper mapper, ICacheManager cacheManager) : IRequestHandler<FindByIdQuery, TemplateDto>
{
    public async Task<TemplateDto> Handle(FindByIdQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var key = TemplateCacheKey.For(request.Id);

        var exists = await cacheManager.ExistsAsync(key);

        if (exists)
            return await cacheManager.GetAsync<TemplateDto>(key);

        var role = await repository.FindAsync<TemplateAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(role, Errors.TemplateNotFound);

        await cacheManager.SetAsync(key, mapper.Map<TemplateDto>(role));

        return mapper.Map<TemplateDto>(role);
    }
}
