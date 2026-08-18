using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Commands.SendEmail;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Models;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Services;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Test.Emails;

/// <summary>
/// Fija el contrato del envio: el cuerpo viaja en base64 y el asunto en texto plano.
/// </summary>
/// <remarks>
/// El asunto se pasaba por <c>BuildBody</c>, que hace <c>Convert.FromBase64String</c>. Con las 14 plantillas
/// sembradas fallaba siempre que hubiera variables que sustituir, y no se envio nunca un correo: ni la
/// bienvenida al aprovisionar, ni la contrasena temporal, ni el codigo para firmar un contrato.
/// </remarks>
public class EnvioDeCorreoTest
{
    private const string Asunto = "Te han invitado a {{organization_name}}";
    private static readonly Dictionary<string, string> Valores = new() { { "organization_name", "Malpelo VI" } };

    [Fact]
    public async Task ElAsuntoSeSustituyeYViajaEnTextoPlano()
    {
        var (handler, sender, enviado) = Construir();

        await handler.Handle(new SendEmailCommand(Guid.NewGuid(), Guid.NewGuid(), ["quien@sea.com"], [], [],
            Asunto, [], Valores), CancellationToken.None);

        Assert.Equal("Te han invitado a Malpelo VI", enviado.Value!.Subject);
        sender.Verify(x => x.BuildSubject(Asunto, Valores), Times.Once);
    }

    [Fact]
    public async Task ElAsuntoNoPasaPorLaConversionDelCuerpo()
    {
        // Es la comprobacion que faltaba: BuildBody trata su entrada como base64, y un asunto no lo es.
        var (handler, sender, _) = Construir();

        await handler.Handle(new SendEmailCommand(Guid.NewGuid(), Guid.NewGuid(), ["quien@sea.com"], [], [],
            Asunto, [], Valores), CancellationToken.None);

        sender.Verify(x => x.BuildBody(Asunto, It.IsAny<Dictionary<string, string>>()), Times.Never);
    }

    [Fact]
    public async Task ElCuerpoSigueViajandoEnBase64()
    {
        var (handler, _, enviado) = Construir();

        await handler.Handle(new SendEmailCommand(Guid.NewGuid(), Guid.NewGuid(), ["quien@sea.com"], [], [],
            Asunto, [], Valores), CancellationToken.None);

        var cuerpo = Encoding.UTF8.GetString(Convert.FromBase64String(enviado.Value!.Body));
        Assert.Contains("Malpelo VI", cuerpo);
    }

    /// <summary>El doble imita al remitente real: BuildBody exige base64 y BuildSubject no.</summary>
    private static (SendEmailCommandHandler, Mock<IEmailSender>, StrongBox<EmailMessage>) Construir()
    {
        var plantilla = TemplateAggregate.Create(Guid.NewGuid(), "InvitationToOrganization", Asunto,
            Convert.ToBase64String(Encoding.UTF8.GetBytes("<p>Hola {{organization_name}}</p>")),
            ["organization_name"], [], "no-reply@kappali.com", "Kappali", true, null, Guid.NewGuid());

        var repositorio = new Mock<IEmailsRepository>();
        repositorio.Setup(x => x.ExistsAsync<EmailsAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repositorio.Setup(x => x.ExistsAsync<TemplateAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        repositorio.Setup(x => x.FindAsync<TemplateAggregate>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plantilla);

        var enviado = new StrongBox<EmailMessage>();
        var sender = new Mock<IEmailSender>();
        sender.Setup(x => x.BuildSubject(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
            .Returns((string s, Dictionary<string, string> v) => Sustituir(s, v));
        sender.Setup(x => x.BuildBody(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
            .Returns((string t, Dictionary<string, string> v) =>
                Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    Sustituir(Encoding.UTF8.GetString(Convert.FromBase64String(t)), v))));
        sender.Setup(x => x.SendEmail(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Callback((EmailMessage m, CancellationToken _) => enviado.Value = m)
            .ReturnsAsync(EmailResponse.Create(null, "202"));

        var contexto = new Mock<IUserContext>();
        contexto.SetupGet(x => x.Tenant).Returns(Guid.NewGuid());

        return (new SendEmailCommandHandler(repositorio.Object, new Mock<IPubSub>().Object,
            contexto.Object, sender.Object), sender, enviado);
    }

    private static string Sustituir(string texto, Dictionary<string, string> valores)
    {
        foreach (var kvp in valores)
            texto = texto.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        return texto;
    }
}

internal sealed class StrongBox<T> { public T? Value { get; set; } }
