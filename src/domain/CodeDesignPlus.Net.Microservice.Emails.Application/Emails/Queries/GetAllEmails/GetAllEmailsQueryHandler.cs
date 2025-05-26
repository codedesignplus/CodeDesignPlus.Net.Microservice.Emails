using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Queries.GetAllEmails;

public class GetAllEmailsQueryHandler(IEmailsRepository repository, IMapper mapper) : IRequestHandler<GetAllEmailsQuery, Pagination<EmailsDto>>
{
    public async Task<Pagination<EmailsDto>> Handle(GetAllEmailsQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var tenants = await repository.MatchingAsync<EmailsAggregate>(request.Criteria, cancellationToken);

        return mapper.Map<Pagination<EmailsDto>>(tenants);
    }
}
