namespace CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.UpdateConfigUserTemplate;

public class UpdateConfigUserTemplateCommandHandler(IUserRepository repository, ITemplateRepository templateRepository) : IRequestHandler<UpdateConfigUserTemplateCommand>
{
    public async Task Handle(UpdateConfigUserTemplateCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var templateExist = await templateRepository.ExistsAsync<TemplateAggregate>(request.IdTemplate, cancellationToken);

        ApplicationGuard.IsFalse(templateExist, Errors.TemplateNotFound);

        var aggregate = await repository.FindAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.UserConfigTemplateNotFound);

        aggregate.UpdateTemplate(request.IdTemplate, request.TypeTemplate, request.Subject, request.UriLoginApp);

        await repository.UpdateAsync(aggregate, cancellationToken);
    }
}