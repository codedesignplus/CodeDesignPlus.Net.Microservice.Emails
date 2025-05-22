using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.Smtp.Application.Template.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Queries.GetAll;

public record GetAllQuery(C.Criteria Criteria) : IRequest<Pagination<TemplateDto>>;

