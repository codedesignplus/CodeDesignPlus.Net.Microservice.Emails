using CodeDesignPlus.Net.Microservice.Emails.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.UpdateTemplate;

/// <summary>
/// Saves the changes of a template and drops the copy that <c>GET Template/{id}</c> keeps in the cache (pendings/674).
/// </summary>
/// <remarks>
/// Without it the next read served the previous version for five minutes, and whoever edited again overwrote their own
/// change with it.
/// </remarks>
public class UpdateTemplateCommandHandler(ITemplateRepository repository, IUserContext user, IPubSub pubsub, ICacheManager cacheManager) : IRequestHandler<UpdateTemplateCommand>
{
    public async Task Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<TemplateAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.TemplateNotFound);

        var attachments = request.Attachments?.Select(a => new FileAttachment(a.Id, a.Name, a.Target)).ToList() ?? [];

        aggregate.Update(request.Name, request.Subject, request.Body, request.Variables, attachments, request.From, request.Alias, request.IsHtml, user.IdUser, request.SensitiveVariables);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await cacheManager.RemoveAsync(TemplateCacheKey.For(aggregate.Id));

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}