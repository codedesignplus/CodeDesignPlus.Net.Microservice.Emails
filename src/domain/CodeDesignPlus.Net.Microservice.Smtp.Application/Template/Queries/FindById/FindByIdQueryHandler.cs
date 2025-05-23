using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.Smtp.Application.Template.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Queries.FindById;

public class FindByIdQueryHandler(ITemplateRepository repository, IMapper mapper, ICacheManager cacheManager) : IRequestHandler<FindByIdQuery, TemplateDto>
{
    public async Task<TemplateDto> Handle(FindByIdQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exists = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exists)
            return await cacheManager.GetAsync<TemplateDto>(request.Id.ToString());

        var role = await repository.FindAsync<TemplateAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(role, Errors.TemplateNotFound);

        await cacheManager.SetAsync(request.Id.ToString(), mapper.Map<TemplateDto>(role));

        return mapper.Map<TemplateDto>(role);
    }
}
