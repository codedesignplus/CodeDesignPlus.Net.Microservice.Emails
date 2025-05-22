namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Emails.Queries.GetAllEmails;

public class GetAllEmailsQueryHandler(IEmailsRepository repository, IMapper mapper, IUserContext user) : IRequestHandler<GetAllEmailsQuery, EmailsDto>
{
    public Task<EmailsDto> Handle(GetAllEmailsQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<EmailsDto>(default!);
    }
}
