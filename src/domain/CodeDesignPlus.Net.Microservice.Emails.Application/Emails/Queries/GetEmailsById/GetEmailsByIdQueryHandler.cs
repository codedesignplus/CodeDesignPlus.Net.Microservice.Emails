namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Queries.GetEmailsById;

public class GetEmailsByIdQueryHandler(IEmailsRepository repository, IMapper mapper, ICacheManager cacheManager) : IRequestHandler<GetEmailsByIdQuery, EmailsDto>
{
    public async Task<EmailsDto> Handle(GetEmailsByIdQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var exists = await cacheManager.ExistsAsync(request.Id.ToString());

        if (exists)
            return await cacheManager.GetAsync<EmailsDto>(request.Id.ToString());

        var tenant = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(tenant, Errors.EmailNotFound);

        var dto = mapper.Map<EmailsDto>(tenant);

        await cacheManager.SetAsync(request.Id.ToString(), dto);

        return dto;
    }
}
