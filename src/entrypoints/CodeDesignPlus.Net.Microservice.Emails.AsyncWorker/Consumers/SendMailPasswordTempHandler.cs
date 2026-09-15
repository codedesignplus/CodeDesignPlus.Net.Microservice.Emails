using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.SendMailPasswordTemp;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Consumers;

/// <summary>
/// Envia la contrasena temporal al usuario recien creado.
/// </summary>
/// <remarks>
/// <b>Sin contrasena no hay correo que mandar, y eso no es un fallo.</b> <c>UserCreatedDomainEvent</c> se
/// emite dos veces para el mismo usuario: al crearlo, con sus claves, y otra vez desde
/// <c>CompleteProviderRegistration</c> cuando Entra le asigna identificador, esa sin claves. La segunda no
/// trae dato nuevo para este consumidor.
/// <para>
/// La guarda de <c>WasCreatedFromSSO</c> no cubre el caso, porque ese indicador dice **como se creo el
/// usuario**, no si este mensaje trae contrasena: un usuario invitado —creado con clave, con el indicador en
/// falso— recibe igualmente la segunda emision, y ahi el comando se rechazaba. Las dos colas de descarte
/// crecian solas, y el intercambio de descarte es un fanout, asi que cada rechazo dejaba copia tambien en la
/// de <c>ms-users</c>, que no tiene nada que ver.
/// </para>
/// <para>
/// De paso evita reenviar la contrasena de alguien que ya la recibio, que es lo que pasaria si el comando
/// llegara a aceptar la segunda emision.
/// </para>
/// </remarks>
[QueueName("User", "sendmailpasswordtemp")]
public class SendMailPasswordTempHandler(IMediator mediator, ILogger<SendMailPasswordTempHandler> logger) : IEventHandler<UserCreatedDomainEvent>
{
    public async Task HandleAsync(UserCreatedDomainEvent data, CancellationToken token)
    {
        if (data.WasCreatedFromSSO)
            return;

        if (string.IsNullOrWhiteSpace(data.PasswordKey) || string.IsNullOrWhiteSpace(data.PasswordCipher))
        {
            logger.LogInformation(
                "El usuario {UserId} llega sin contrasena temporal, asi que no hay correo que enviar. Es el reanuncio del registro en el proveedor de identidad, no un alta.",
                data.AggregateId);

            return;
        }

        var command = new SendMailPasswordTempCommand(
            data.AggregateId,
            data.FirstName,
            data.LastName,
            data.DisplayName,
            data.Email,
            data.Phone,
            data.PasswordKey,
            data.PasswordCipher,
            data.IsActive
        );

        await mediator.Send(command, token);
    }
}
