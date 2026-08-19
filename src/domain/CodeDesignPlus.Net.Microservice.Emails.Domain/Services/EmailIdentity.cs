using System.Security.Cryptography;
using System.Text;

namespace CodeDesignPlus.Net.Microservice.Emails.Domain.Services;

/// <summary>
/// Construye el identificador de un correo a partir de lo que lo hace unico.
/// </summary>
/// <remarks>
/// <para>
/// El envio es idempotente por identificador: si ya existe un correo con ese id, se rechaza como error de
/// negocio y no se reintenta. Eso protege de que una reentrega duplique el mensaje, pero solo funciona si el
/// identificador describe <b>el correo</b> y no una de sus partes.
/// </para>
/// <para>
/// La bienvenida a una copropiedad usaba el id del usuario a secas. El mismo administrador dando de alta su
/// segunda copropiedad chocaba con el correo de la primera y <b>nunca recibia el aviso</b>: el mensaje salia
/// rechazado y moria en su cola de descarte, sin que nadie lo notara.
/// </para>
/// </remarks>
public static class EmailIdentity
{
    private static readonly Guid WelcomeToTenantNamespace = Guid.Parse("2f4a1c07-6b8e-4d31-9a52-c0e7b5d81f36");

    /// <summary>
    /// El identificador de la bienvenida de un usuario a una copropiedad concreta.
    /// </summary>
    /// <remarks>
    /// Es estable: la misma pareja da siempre el mismo identificador, asi que una reentrega sigue siendo
    /// idempotente. Y es distinto por copropiedad, que es lo que faltaba.
    /// </remarks>
    public static Guid ForWelcomeToTenant(Guid userId, Guid tenantId)
        => Create(WelcomeToTenantNamespace, $"{userId:N}|{tenantId:N}");

    /// <summary>Identificador determinista basado en nombre (UUID version 5, RFC 4122 seccion 4.3).</summary>
    public static Guid Create(Guid namespaceId, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var namespaceBytes = namespaceId.ToByteArray();
        SwapByteOrder(namespaceBytes);

        var nameBytes = Encoding.UTF8.GetBytes(name);

        var payload = new byte[namespaceBytes.Length + nameBytes.Length];
        namespaceBytes.CopyTo(payload, 0);
        nameBytes.CopyTo(payload, namespaceBytes.Length);

        // SHA-1 lo impone el RFC para la version 5. No se usa aqui con proposito criptografico.
        var hash = SHA1.HashData(payload);

        var result = new byte[16];
        Array.Copy(hash, 0, result, 0, 16);

        result[6] = (byte)((result[6] & 0x0F) | 0x50);
        result[8] = (byte)((result[8] & 0x3F) | 0x80);

        SwapByteOrder(result);

        return new Guid(result);
    }

    private static void SwapByteOrder(byte[] guid)
    {
        (guid[0], guid[3]) = (guid[3], guid[0]);
        (guid[1], guid[2]) = (guid[2], guid[1]);
        (guid[4], guid[5]) = (guid[5], guid[4]);
        (guid[6], guid[7]) = (guid[7], guid[6]);
    }
}
