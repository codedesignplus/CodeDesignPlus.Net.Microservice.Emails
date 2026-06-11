using CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.GetSystemTemplates;

public record GetSystemTemplatesQuery() : IRequest<List<TemplateDto>>;
