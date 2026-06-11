using CodeDesignPlus.Net.Microservice.Emails.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.UpdateTemplate;

public class UpdateTemplateCommandHandler(ITemplateRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<UpdateTemplateCommand>
{
    public async Task Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<TemplateAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.TemplateNotFound);

        var attachments = request.Attachments?.Select(a => new FileAttachment(a.Id, a.Name, a.Target)).ToList() ?? [];

        aggregate.Update(request.Name, request.Subject, request.Body, request.Variables, attachments, request.From, request.Alias, request.IsHtml, user.IdUser);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}