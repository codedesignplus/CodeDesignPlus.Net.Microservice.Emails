using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Commands.SendEmail;
using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.SendMailWelcomeToTenant;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Enums;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Test.User;

/// <summary>
/// Whoever buys a tenant was not invited to it: they get "your tenant is ready" instead of the invitation
/// (pendings/069).
/// </summary>
public class SendMailWelcomeToTenantCommandHandlerTest
{
    private static readonly Guid SystemUser = Guid.Parse("10000000-0000-0000-0000-000000000001");

    [Theory]
    [InlineData(true, nameof(TypeTemplate.TenantReady))]
    [InlineData(false, nameof(TypeTemplate.InvitationToOrganization))]
    public async Task Handle_ByPurchaseOrNot_SendsTheMatchingTemplate(bool byPurchase, string expectedTemplate)
    {
        var template = TemplateAggregate.Create(
            Guid.NewGuid(), expectedTemplate, "Subject", "<p>Body</p>", ["display_name"], [],
            "noreply@example.com", "Kappali", true, null, SystemUser);

        var repository = new Mock<ITemplateRepository>();
        repository
            .Setup(r => r.FindByNameAndTenantAsync(expectedTemplate, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        SendEmailCommand? sent = null;
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(m => m.Send(It.IsAny<SendEmailCommand>(), It.IsAny<CancellationToken>()))
            .Callback<SendEmailCommand, CancellationToken>((c, _) => sent = c)
            .Returns(Task.CompletedTask);

        var handler = new SendMailWelcomeToTenantCommandHandler(repository.Object, mediator.Object);
        var command = new SendMailWelcomeToTenantCommand(
            Guid.NewGuid(), "buyer@example.com", "Buyer", Guid.NewGuid(), "Conjunto Prueba", byPurchase);

        await handler.Handle(command, CancellationToken.None);

        repository.Verify(r => r.FindByNameAndTenantAsync(expectedTemplate, null, It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(sent);
        Assert.Equal(template.Id, sent!.IdTemplate);
    }

    [Fact]
    public async Task Handle_TenantReadyTemplateMissing_ThrowsItsOwnError()
    {
        var repository = new Mock<ITemplateRepository>();
        var handler = new SendMailWelcomeToTenantCommandHandler(repository.Object, new Mock<IMediator>().Object);
        var command = new SendMailWelcomeToTenantCommand(
            Guid.NewGuid(), "buyer@example.com", "Buyer", Guid.NewGuid(), "Conjunto Prueba", ByPurchase: true);

        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Equal(Errors.TemplateTenantReadyNotFound.GetCode(), exception.Code);
    }
}
