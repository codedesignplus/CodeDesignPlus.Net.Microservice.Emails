namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Commands.CreateTemplate;

public class CreateTemplateCommandHandler(ITemplateRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<CreateTemplateCommand>
{
    public async Task Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        var exist = await repository.ExistsAsync<TemplateAggregate>(request.Id, user.Tenant, cancellationToken);

        ApplicationGuard.IsTrue(exist, Errors.TemplateAlreadyExists);

        var aggregate = TemplateAggregate.Create(request.Id, request.Name, request.Subject, request.Body, request.Variables, request.Attachments, request.From, request.Alias, request.IsHtml, user.Tenant, user.IdUser);

        await repository.CreateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}