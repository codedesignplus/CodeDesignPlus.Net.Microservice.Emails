namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.DeleteTemplate;

public class DeleteTemplateCommandHandler(ITemplateRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<DeleteTemplateCommand>
{
    public async Task Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
    {        
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<TemplateAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.TemplateNotFound);

        aggregate.Delete(user.IdUser);

        await repository.DeleteAsync<TemplateAggregate>(aggregate.Id, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}