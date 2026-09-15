using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.SendMailPasswordTemp;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Test.Consumers;

/// <summary>
/// Cubre cuando se manda la contrasena temporal y cuando no.
/// </summary>
/// <remarks>
/// <c>UserCreatedDomainEvent</c> se emite dos veces para el mismo usuario: al crearlo, con sus claves, y otra
/// vez desde <c>CompleteProviderRegistration</c> cuando Entra le asigna identificador, esa sin claves.
/// <para>
/// La segunda rechazaba, porque el validador del comando exige la contrasena y la guarda solo miraba
/// <c>WasCreatedFromSSO</c>, que dice como se creo el usuario y no si el mensaje trae clave. Las dos colas de
/// descarte crecian solas —cuatro mensajes en hora y media— y el fanout del descarte duplicaba cada rechazo
/// en la cola de <c>ms-users</c>, que no tiene nada que ver.
/// </para>
/// </remarks>
public class SendMailPasswordTempHandlerTest
{
    private static readonly Guid UserId = Guid.Parse("175174a0-c4c2-41a5-9fc3-69a790020543");

    private readonly Mock<IMediator> mediator = new();
    private readonly List<SendMailPasswordTempCommand> enviados = [];

    public SendMailPasswordTempHandlerTest()
    {
        mediator
            .Setup(x => x.Send(It.IsAny<SendMailPasswordTempCommand>(), It.IsAny<CancellationToken>()))
            .Callback<IRequest, CancellationToken>((c, _) => enviados.Add((SendMailPasswordTempCommand)c))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task ElAltaConClaveSiMandaElCorreo()
    {
        await Handle(passwordKey: "una-clave", passwordCipher: "un-cifrado", wasCreatedFromSSO: false);

        var comando = Assert.Single(enviados);

        Assert.Equal(UserId, comando.Id);
        Assert.Equal("una-clave", comando.PasswordKey);
        Assert.Equal("un-cifrado", comando.PasswordCipher);
    }

    [Fact]
    public async Task ElReanuncioSinClaveNoMandaNadaNiRechaza()
    {
        // Es el caso que llenaba las colas de descarte: usuario invitado, creado con clave, que recibe la
        // segunda emision cuando Entra le asigna identificador. No es un alta, asi que no hay correo.
        await Handle(passwordKey: null, passwordCipher: null, wasCreatedFromSSO: false);

        Assert.Empty(enviados);
    }

    [Theory]
    [InlineData("", "un-cifrado")]
    [InlineData("una-clave", "")]
    [InlineData("   ", "   ")]
    public async Task UnaClaveAMediasTampocoMandaNada(string key, string cipher)
    {
        // Media contrasena no sirve para nada, y el validador del comando la rechazaria igual.
        await Handle(passwordKey: key, passwordCipher: cipher, wasCreatedFromSSO: false);

        Assert.Empty(enviados);
    }

    [Fact]
    public async Task UnUsuarioDeSsoNoRecibeContrasenaTemporal()
    {
        // Su identidad la gobierna Entra: no hay ninguna clave que comunicarle.
        await Handle(passwordKey: null, passwordCipher: null, wasCreatedFromSSO: true);

        Assert.Empty(enviados);
    }

    private Task Handle(string? passwordKey, string? passwordCipher, bool wasCreatedFromSSO)
    {
        var handler = new SendMailPasswordTempHandler(mediator.Object, Mock.Of<ILogger<SendMailPasswordTempHandler>>());

        var evento = UserCreatedDomainEvent.Create(
            UserId, "Andres", "Flipe", "wliscano+afgomez@codedesignplus.com", "+573001112201",
            "Andres Flipe", passwordKey, passwordCipher, wasCreatedFromSSO, true);

        return handler.HandleAsync(evento, CancellationToken.None);
    }
}
