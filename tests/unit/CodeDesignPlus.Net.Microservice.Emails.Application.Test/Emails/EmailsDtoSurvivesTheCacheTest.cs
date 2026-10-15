using System.Collections.Generic;
using System.Linq;
using CodeDesignPlus.Net.Microservice.Emails.Application.Setup;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Models;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Test.Emails;

/// <summary>
/// El detalle de un correo vuelve del cache igual que entró (pendings/381).
/// </summary>
/// <remarks>
/// <c>GetEmailsByIdQueryHandler</c> guarda el detalle en Redis con el serializador del SDK (Newtonsoft), que solo
/// asigna las propiedades con <c>set</c> público. <c>EmailsDto</c> las tenía todas con <c>set</c> privado: a partir de
/// la segunda lectura el correo volvía sin destinatarios, asunto, cuerpo ni remitente.
/// </remarks>
public class EmailsDtoSurvivesTheCacheTest
{
    private static readonly Guid Tenant = Guid.Parse("17daab5e-b090-48bd-969a-a7618eb2a726");

    [Fact]
    public void RoundTrip_SentEmailDetail_ComesBackAsItWentIn()
    {
        // Arrange
        var dto = Mapper().Map<EmailsDto>(SentEmail());

        // Act
        var cached = JsonSerializer.Deserialize<EmailsDto>(JsonSerializer.Serialize(dto));

        // Assert
        Assert.Equal(Describe(dto), Describe(cached));
    }

    [Fact]
    public void Map_SentEmail_KeepsEveryFieldAndTheAttachmentNames()
    {
        // Act
        var dto = Mapper().Map<EmailsDto>(SentEmail());

        // Assert
        Assert.Equal("ana@kappali.com|conta@kappali.com|auditor@kappali.com|Cuenta de cobro CC-000123|PGgxPkhvbGE8L2gxPg==|no-reply@kappali.com|CC-000123.pdf|True|unit=T1-101|invoice|-|" + Tenant, Describe(dto));
    }

    private static string Describe(EmailsDto dto)
    {
        return string.Join('|', string.Join(',', dto.To), string.Join(',', dto.Cc), string.Join(',', dto.Bcc), dto.Subject, dto.Body, dto.From, string.Join(',', dto.Attachments), dto.IsHtml, string.Join(',', dto.Values.Select(x => $"{x.Key}={x.Value}")), dto.Code, dto.Error ?? "-", dto.Tenant);
    }

    private static EmailsAggregate SentEmail()
    {
        return EmailsAggregate.Create(Guid.NewGuid(), ["ana@kappali.com"], ["conta@kappali.com"], ["auditor@kappali.com"], "Cuenta de cobro CC-000123", "PGgxPkhvbGE8L2gxPg==", "no-reply@kappali.com", [Attachment.Create("CC-000123.pdf", "application/pdf", [1, 2, 3])], true, new Dictionary<string, string> { { "unit", "T1-101" } }, "invoice", null, Tenant);
    }

    private static Mapper Mapper()
    {
        MapsterConfigEmails.Configure();

        return new Mapper(TypeAdapterConfig.GlobalSettings);
    }
}
