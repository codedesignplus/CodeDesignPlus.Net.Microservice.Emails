using CodeDesignPlus.Net.Microservice.Emails.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.CreateTemplate;

public class CreateTemplateCommandHandler(ITemplateRepository repository, IUserContext user, IPubSub pubsub) : IRequestHandler<CreateTemplateCommand>
{
    public async Task Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        // Ya guardado por un envío anterior del mismo formulario: el reintento termina bien sin repetir nada (pendings/589).
        if (await repository.ExistsAsync<TemplateAggregate>(request.Id, user.Tenant, cancellationToken))
            return;

        var attachments = request.Attachments?.Select(a => new FileAttachment(a.Id, a.Name, a.Target)).ToList() ?? [];

        var aggregate = TemplateAggregate.Create(request.Id, request.Name, request.Subject, request.Body, request.Variables, attachments, request.From, request.Alias, request.IsHtml, user.Tenant, user.IdUser);

        // Dos envíos que llegan a la vez pasan los dos la comprobación de arriba; el segundo choca contra el id y no publica.
        if (!await repository.CreateIfAbsentAsync(aggregate, cancellationToken))
            return;

        await pubsub.PublishAsync(aggregate.GetAndClearEvents(), cancellationToken);
    }
}