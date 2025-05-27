namespace CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.CreateConfigUserTemplate;

public class CreateConfigUserTemplateCommandHandler(IUserRepository repository, ITemplateRepository templateRepository) : IRequestHandler<CreateConfigUserTemplateCommand>
{
    public async Task Handle(CreateConfigUserTemplateCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);
            
        var exist = await repository.ExistsAsync<UserAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.UserConfigTemplateAlreadyExists);

        var templateExist = await templateRepository.ExistsAsync<TemplateAggregate>(request.IdTemplate, cancellationToken);

        ApplicationGuard.IsFalse(templateExist, Errors.TemplateNotFound);

        var aggregate = UserAggregate.Create(request.Id, request.IdTemplate, request.TypeTemplate, request.Subject, request.UriLoginApp);

        await repository.CreateAsync(aggregate, cancellationToken);
    }
}