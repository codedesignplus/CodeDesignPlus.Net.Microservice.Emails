using CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.GetSystemTemplates;

public class GetSystemTemplatesQueryHandler(ITemplateRepository repository, IMapper mapper) : IRequestHandler<GetSystemTemplatesQuery, List<TemplateDto>>
{
    public async Task<List<TemplateDto>> Handle(GetSystemTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await repository.GetSystemTemplatesAsync(cancellationToken);

        return mapper.Map<List<TemplateDto>>(templates);
    }
}
