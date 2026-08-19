using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.SendMailWelcomeToTenant;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Services;
using MediatR;

namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Test.Consumers;

/// <summary>
/// Cubre el identificador de la bienvenida a una copropiedad.
/// </summary>
/// <remarks>
/// El envio es idempotente por identificador, y este usaba el del usuario a secas. El mismo administrador
/// dando de alta su segunda copropiedad chocaba con la bienvenida de la primera: el correo se rechazaba como
/// error de negocio, moria en su cola de descarte y <b>nunca llegaba</b>. Medido el 2026-08-19: de tres
/// copropiedades creadas con la misma cuenta solo la primera recibio aviso.
/// </remarks>
public class SendMailWelcomeToTenantHandlerTest
{
    private static readonly Guid Usuario = Guid.Parse("58869df7-18a5-4980-af64-b37d57843240");
    private static readonly Guid Malpelo = Guid.Parse("cac3ecb2-519b-4619-bd96-230118fcc182");
    private static readonly Guid Otra = Guid.Parse("fdd6fa8e-744c-43fa-936a-bc7e07db5875");

    [Fact]
    public async Task ElMismoUsuarioEnDosCopropiedadesRecibeDosCorreos()
    {
        var enviados = new List<SendMailWelcomeToTenantCommand>();
        var mediator = Mediator(enviados);

        await Handler(mediator).HandleAsync(Evento(Malpelo, "Malpelo X"), CancellationToken.None);
        await Handler(mediator).HandleAsync(Evento(Otra, "Malpelo XI"), CancellationToken.None);

        Assert.Equal(2, enviados.Count);
        Assert.NotEqual(enviados[0].Id, enviados[1].Id);
    }

    [Fact]
    public async Task ElIdentificadorNoEsElDelUsuario()
    {
        // Era el fallo exacto: el correo se identificaba por quien lo recibe y no por lo que anuncia.
        var enviados = new List<SendMailWelcomeToTenantCommand>();

        await Handler(Mediator(enviados)).HandleAsync(Evento(Malpelo, "Malpelo X"), CancellationToken.None);

        Assert.NotEqual(Usuario, Assert.Single(enviados).Id);
    }

    [Fact]
    public async Task ReprocesarElMismoEventoNoDuplicaElCorreo()
    {
        // La idempotencia tiene que seguir en pie: una reentrega debe dar el mismo identificador para que el
        // envio la rechace en vez de mandar el aviso dos veces.
        var enviados = new List<SendMailWelcomeToTenantCommand>();
        var mediator = Mediator(enviados);

        await Handler(mediator).HandleAsync(Evento(Malpelo, "Malpelo X"), CancellationToken.None);
        await Handler(mediator).HandleAsync(Evento(Malpelo, "Malpelo X"), CancellationToken.None);

        Assert.Equal(enviados[0].Id, enviados[1].Id);
    }

    [Fact]
    public void ElIdentificadorEsEstableEntreProcesos()
    {
        // Si dependiera del momento o del proceso, un reinicio a media entrega mandaria el correo otra vez.
        Assert.Equal(
            EmailIdentity.ForWelcomeToTenant(Usuario, Malpelo),
            EmailIdentity.ForWelcomeToTenant(Usuario, Malpelo));
    }

    private static SendMailWelcomeToTenantHandler Handler(IMediator mediator) => new(mediator);

    private static IMediator Mediator(List<SendMailWelcomeToTenantCommand> sink)
    {
        var mediator = new Mock<IMediator>();

        mediator
            .Setup(x => x.Send(It.IsAny<SendMailWelcomeToTenantCommand>(), It.IsAny<CancellationToken>()))
            .Callback<SendMailWelcomeToTenantCommand, CancellationToken>((c, _) => sink.Add(c))
            .Returns(Task.CompletedTask);

        return mediator.Object;
    }

    private static TenantAddedDomainEvent Evento(Guid tenantId, string nombre)
        => new(Usuario, "wliscano+adm@codedesignplus.com", "Wilzon Liscano",
            new TenantInfo { Id = tenantId, Name = nombre });
}
