namespace CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Commands.UpdateTemplate;

public class UpdateTemplateCommandHandler(ITemplateRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<UpdateTemplateCommand>
{
    public async Task Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
        ApplicationGuard.IsNull(request, Errors.InvalidRequest);

        var aggregate = await repository.FindAsync<TemplateAggregate>(request.Id, cancellationToken);

        ApplicationGuard.IsNull(aggregate, Errors.TemplateNotFound);

        aggregate.Update(request.Name, request.Subject, request.Body, request.Variables, request.Attachments, request.From, request.Alias, request.IsHtml, user.Tenant);

        await repository.UpdateAsync(aggregate, cancellationToken);

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}