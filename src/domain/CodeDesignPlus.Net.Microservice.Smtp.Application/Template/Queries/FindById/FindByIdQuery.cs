using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.Smtp.Application.Template.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Queries.FindById;

public record FindByIdQuery(Guid Id) : IRequest<TemplateDto>;

