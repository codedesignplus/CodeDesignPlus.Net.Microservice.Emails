namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.DeleteTemplate;

/// <summary>
/// Deletes a template and drops the copy that <c>GET Template/{id}</c> keeps in the cache, so it is not read after it is gone
/// (pendings/674).
/// </summary>
public class DeleteTemplateCommandHandler(ITemplateRepository repository, IUserContext user, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<DeleteTemplateCommand>
{
    public async Task Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
    {        
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<TemplateAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.TemplateNotFound);

        aggregate.Delete(user.IdUser);

        await repository.DeleteAsync<TemplateAggregate>(aggregate.Id, cancellationToken);

        await cacheManager.RemoveAsync(TemplateCacheKey.For(aggregate.Id));

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}