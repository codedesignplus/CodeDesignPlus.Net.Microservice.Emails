namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Queries.GetEmailsById;

public record GetEmailsByIdQuery(Guid Id) : IRequest<EmailsDto>;

