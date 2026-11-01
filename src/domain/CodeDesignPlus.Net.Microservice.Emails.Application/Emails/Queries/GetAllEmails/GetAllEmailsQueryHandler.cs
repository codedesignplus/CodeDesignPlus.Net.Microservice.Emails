using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Queries.GetAllEmails;

public class GetAllEmailsQueryHandler(IEmailsRepository repository, IMapper mapper, IUserContext user) : IRequestHandler<GetAllEmailsQuery, Pagination<EmailsDto>>
{
    public async Task<Pagination<EmailsDto>> Handle(GetAllEmailsQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        // Sin copropiedad no hay historial que mostrar: los correos de la plataforma no se leen desde aquí (pendings/680).
        if (user.Tenant == Guid.Empty)
            return Pagination<EmailsDto>.Create([], 0, limit: request.Criteria?.Limit, skip: request.Criteria?.Skip);

        var emails = await repository.MatchingAsync<EmailsAggregate>(TenantScopedCriteria.Scope(user.Tenant, request.Criteria!), cancellationToken);

        return mapper.Map<Pagination<EmailsDto>>(emails);
    }
}
