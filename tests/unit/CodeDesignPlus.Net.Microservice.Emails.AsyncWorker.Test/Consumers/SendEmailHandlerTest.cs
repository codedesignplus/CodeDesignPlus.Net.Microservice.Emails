using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.File.Storage.Abstractions;
using CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Commands.SendEmail;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Emails.Domain;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Repositories;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Test.Consumers;

/// <summary>
/// El correo que pide una copropiedad por evento se guarda con su copropiedad (pendings/680).
/// </summary>
/// <remarks>
/// El contexto del worker no tiene copropiedad: sin pasar la del evento, todos los correos quedaban con
/// <see cref="Guid.Empty"/> y ninguna copropiedad veía los suyos en el historial.
/// </remarks>
public class SendEmailHandlerTest
{
    private static readonly Guid Tenant = Guid.Parse("274bb0a1-0000-0000-0000-000000000001");

    [Fact]
    public async Task HandleAsync_EventOfATenant_SendsTheCommandWithItsTenant()
    {
        // Arrange
        var template = TemplateAggregate.Create(Guid.NewGuid(), "Notice", "{{subject}}", "<p>{{content}}</p>", ["subject", "content"], [], "no-reply@kappali.com", "Kappali", true, Tenant, Guid.NewGuid());

        var templates = new Mock<ITemplateRepository>();
        templates.Setup(x => x.FindByNameAndTenantAsync("Notice", Tenant, It.IsAny<CancellationToken>())).ReturnsAsync(template);

        SendEmailCommand? sent = null;
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<SendEmailCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest, CancellationToken>((c, _) => sent = (SendEmailCommand)c)
            .Returns(Task.CompletedTask);

        var handler = new SendEmailHandler(mediator.Object, new Mock<IFileStorage>().Object, templates.Object, Mock.Of<ILogger<SendEmailHandler>>());

        var data = new SendEmailDomainEvent(Guid.NewGuid(), "Notice", ["ana@kappali.com"], [], [],
            new Dictionary<string, string> { { "subject", "Asamblea" }, { "content", "Convocatoria" } }, [], Tenant);

        // Act
        await handler.HandleAsync(data, CancellationToken.None);

        // Assert
        Assert.Equal(Tenant, sent!.Tenant);
    }
}
