namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Queries.GetEmailsById;

public class GetEmailsByIdQueryHandler(IEmailsRepository repository, IMapper mapper, ICacheManager cacheManager, IUserContext user) : IRequestHandler<GetEmailsByIdQuery, EmailsDto>
{
    public async Task<EmailsDto> Handle(GetEmailsByIdQuery request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        // La clave lleva la copropiedad: con solo el id, un detalle cacheado se servía a cualquiera (pendings/680).
        var key = $"{user.Tenant}:{request.Id}";

        var exists = await cacheManager.ExistsAsync(key);

        if (exists)
            return await cacheManager.GetAsync<EmailsDto>(key);

        var email = await repository.FindAsync<EmailsAggregate>(request.Id, cancellationToken);

        // Un correo de otra copropiedad o de la plataforma responde igual que uno que no existe.
        ApplicationGuard.IsTrue(email is null || user.Tenant == Guid.Empty || email.Tenant != user.Tenant, Errors.EmailNotFound);

        var dto = mapper.Map<EmailsDto>(email);

        await cacheManager.SetAsync(key, dto);

        return dto;
    }
}
