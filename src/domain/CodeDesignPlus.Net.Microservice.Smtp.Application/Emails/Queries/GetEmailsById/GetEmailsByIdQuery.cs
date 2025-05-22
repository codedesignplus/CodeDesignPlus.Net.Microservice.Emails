namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Emails.Queries.GetEmailsById;

public record GetEmailsByIdQuery(Guid Id) : IRequest<EmailsDto>;

