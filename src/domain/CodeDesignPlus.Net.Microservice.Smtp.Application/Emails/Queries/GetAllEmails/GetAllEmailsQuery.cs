namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Emails.Queries.GetAllEmails;

public record GetAllEmailsQuery(Guid Id) : IRequest<EmailsDto>;

