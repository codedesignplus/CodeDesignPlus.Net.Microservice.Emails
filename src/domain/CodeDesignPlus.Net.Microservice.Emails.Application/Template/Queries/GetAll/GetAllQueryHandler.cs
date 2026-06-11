using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.GetAll;

public class GetAllQueryHandler(ITemplateRepository repository, IMapper mapper, IUserContext userContext) : IRequestHandler<GetAllQuery, Pagination<TemplateDto>>
{
    public async Task<Pagination<TemplateDto>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var templates = await repository.GetByTenantAsync(userContext.Tenant, cancellationToken);

        var mapped = mapper.Map<List<TemplateDto>>(templates);

        return Pagination<TemplateDto>.Create(mapped, mapped.Count, 0, mapped.Count);
    }
}
