using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Commands.SendEmail;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Models;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Test.Emails;

/// <summary>
/// El correo sale con los valores reales, y el historial guarda los sensibles enmascarados, con la copropiedad del envío y
/// el estado que devolvió el proveedor (pendings/680).
/// </summary>
/// <remarks>
/// Antes la contraseña temporal quedaba en claro en <c>Values</c> y en el cuerpo guardado, y el detalle del historial la
/// mostraba. Los correos del worker quedaban sin copropiedad, y todos se guardaban como fallidos.
/// </remarks>
public class SensitiveVariablesAreNotStoredTest
{
    private const string Password = "Kp-7rT!q2";
    private static readonly Guid Tenant = Guid.Parse("274bb0a1-0000-0000-0000-000000000001");

    private readonly Mock<IEmailsRepository> repository = new();
    private readonly Mock<IEmailSender> sender = new();
    private EmailMessage? sent;
    private EmailsAggregate? stored;

    public SensitiveVariablesAreNotStoredTest()
    {
        var template = TemplateAggregate.Create(Guid.NewGuid(), "PasswordTemp", "Tu contraseña temporal - {{display_name}}",
            Convert.ToBase64String(Encoding.UTF8.GetBytes("<p>Hola {{display_name}}, tu clave es {{password}}</p>")),
            ["display_name", "password"], [], "no-reply@kappali.com", "Kappali", true, null, Guid.NewGuid(), ["password"]);

        repository.Setup(x => x.ExistsAsync<EmailsAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        repository.Setup(x => x.ExistsAsync<TemplateAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        repository.Setup(x => x.FindAsync<TemplateAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(template);
        repository.Setup(x => x.CreateAsync(It.IsAny<EmailsAggregate>(), It.IsAny<CancellationToken>()))
            .Callback((EmailsAggregate e, CancellationToken _) => stored = e)
            .Returns(Task.CompletedTask);

        sender.Setup(x => x.BuildSubject(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
            .Returns((string s, Dictionary<string, string> v) => Replace(s, v));
        sender.Setup(x => x.BuildBody(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
            .Returns((string t, Dictionary<string, string> v) =>
                Convert.ToBase64String(Encoding.UTF8.GetBytes(Replace(Encoding.UTF8.GetString(Convert.FromBase64String(t)), v))));
    }

    [Fact]
    public async Task Handle_SensitiveVariable_IsSentButNotStored()
    {
        // Arrange
        Accepts();

        // Act
        await Send(tenant: null);

        // Assert
        Assert.Contains(Password, Decode(sent!.Body));
        Assert.Equal(SendEmailCommandHandler.MaskedValue, stored!.Values["password"]);
        Assert.Equal("Laura Gómez", stored.Values["display_name"]);
        Assert.DoesNotContain(Password, Decode(stored.Body));
        Assert.Contains(SendEmailCommandHandler.MaskedValue, Decode(stored.Body));
    }

    [Fact]
    public async Task Handle_TenantOfTheEvent_IsTheTenantOfTheStoredEmail()
    {
        // Arrange
        Accepts();

        // Act
        await Send(tenant: Tenant);

        // Assert
        Assert.Equal(Tenant, stored!.Tenant);
    }

    [Fact]
    public async Task Handle_AcceptedByTheProvider_IsStoredAsSucceeded()
    {
        // Arrange
        Accepts();

        // Act
        await Send(tenant: Tenant);

        // Assert
        Assert.True(stored!.Succeeded);
        Assert.Null(stored.Error);
        Assert.Equal("202", stored.Code);
    }

    [Fact]
    public async Task Handle_RejectedByTheProvider_IsStoredAsFailedWithItsError()
    {
        // Arrange
        sender.Setup(x => x.SendEmail(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(EmailResponse.Failure("The recipient mailbox is full.", "ErrorQuotaExceeded"));

        // Act
        await Send(tenant: Tenant);

        // Assert
        Assert.False(stored!.Succeeded);
        Assert.Equal("The recipient mailbox is full.", stored.Error);
        Assert.Equal("ErrorQuotaExceeded", stored.Code);
    }

    private void Accepts() =>
        sender.Setup(x => x.SendEmail(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Callback((EmailMessage m, CancellationToken _) => sent = m)
            .ReturnsAsync(EmailResponse.Success("202"));

    private Task Send(Guid? tenant)
    {
        var user = new Mock<IUserContext>();
        user.SetupGet(x => x.Tenant).Returns(Guid.Empty);

        var handler = new SendEmailCommandHandler(repository.Object, new Mock<IPubSub>().Object, user.Object, sender.Object);

        return handler.Handle(new SendEmailCommand(Guid.NewGuid(), Guid.NewGuid(), ["laura@kappali.com"], [], [], "-", [],
            new Dictionary<string, string> { { "display_name", "Laura Gómez" }, { "password", Password } }, tenant), CancellationToken.None);
    }

    private static string Decode(string base64) => Encoding.UTF8.GetString(Convert.FromBase64String(base64));

    private static string Replace(string text, Dictionary<string, string> values)
    {
        foreach (var kvp in values)
            text = text.Replace("{{" + kvp.Key + "}}", kvp.Value);

        return text;
    }
}
