using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.GetAll;

public class GetAllQueryHandler(ITemplateRepository repository, IMapper mapper) : IRequestHandler<GetAllQuery, Pagination<TemplateDto>>
{
    public async Task<Pagination<TemplateDto>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var templates = await repository.MatchingAsync<TemplateAggregate>(request.Criteria, cancellationToken);

        return mapper.Map<Pagination<TemplateDto>>(templates);
    }
}
