using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Queries.GetAllEmails;

public record GetAllEmailsQuery(C.Criteria Criteria) : IRequest<Pagination<EmailsDto>>;

