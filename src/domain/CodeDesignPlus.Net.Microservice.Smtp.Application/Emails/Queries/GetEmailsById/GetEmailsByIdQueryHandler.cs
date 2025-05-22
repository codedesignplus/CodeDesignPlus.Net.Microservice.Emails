namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Emails.Queries.GetEmailsById;

public class GetEmailsByIdQueryHandler(IEmailsRepository repository, IMapper mapper, IUserContext user) : IRequestHandler<GetEmailsByIdQuery, EmailsDto>
{
    public Task<EmailsDto> Handle(GetEmailsByIdQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<EmailsDto>(default!);
    }
}
