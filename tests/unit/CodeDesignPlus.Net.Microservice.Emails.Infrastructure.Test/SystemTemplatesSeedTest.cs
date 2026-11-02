using System.Text.Json;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Enums;
using CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Seeds;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Test;

/// <summary>
/// La semilla de plantillas del sistema trae una plantilla genérica de avisos: el asunto y el contenido los pone quien
/// envía, así que cualquier micro puede mandar un aviso por correo sin que este tenga una plantilla suya.
/// </summary>
public class SystemTemplatesSeedTest
{
    [Fact]
    public void Seed_NoticeTemplate_TakesSubjectAndContentFromTheSender()
    {
        // Arrange
        var resource = typeof(SystemTemplateSeedService).Assembly.GetManifestResourceNames().Single(x => x.EndsWith("system-templates.json", StringComparison.Ordinal));
        using var stream = typeof(SystemTemplateSeedService).Assembly.GetManifestResourceStream(resource)!;

        // Act
        var notice = JsonDocument.Parse(stream).RootElement.EnumerateArray().Single(x => x.GetProperty("name").GetString() == "Notice");

        // Assert
        Assert.Equal(("{{subject}}", true), (notice.GetProperty("subject").GetString(), notice.GetProperty("body").GetString()!.Contains("{{content}}", StringComparison.Ordinal)));
    }

    /// <summary>
    /// El recibo de una renovación o un cambio de licencia muestra el precio, el crédito aplicado y el total pagado
    /// (pendings/738); ms-licenses lo pide por su nombre, que tiene que ser un tipo de plantilla para poder renderizarse.
    /// </summary>
    [Fact]
    public void Seed_SubscriptionReceipt_ShowsPriceCreditAppliedAndAmountPaid()
    {
        // Arrange
        var templates = Templates();

        // Act
        var receipt = templates.Single(x => x.GetProperty("name").GetString() == "SubscriptionReceipt");
        var body = receipt.GetProperty("body").GetString()!;

        // Assert
        Assert.True(Enum.TryParse<TypeTemplate>("SubscriptionReceipt", out _) && new[] { "{{price}}", "{{credit_applied}}", "{{amount_paid}}" }.All(x => body.Contains(x, StringComparison.Ordinal)));
    }

    /// <summary>
    /// Cada variable que usa el recibo de suscripción está declarada en él: la pantalla de plantillas las ofrece desde ahí.
    /// </summary>
    [Fact]
    public void Seed_SubscriptionReceiptPlaceholders_AreDeclared()
    {
        // Arrange
        var receipt = Templates().Single(x => x.GetProperty("name").GetString() == "SubscriptionReceipt");
        var declared = receipt.GetProperty("variables").EnumerateArray().Select(x => x.GetString()).ToHashSet();

        // Act
        var undeclared = System.Text.RegularExpressions.Regex.Matches(receipt.GetProperty("body").GetString()!, @"\{\{(\w+)\}\}")
            .Select(match => match.Groups[1].Value)
            .Where(name => !declared.Contains(name))
            .ToList();

        // Assert
        Assert.Empty(undeclared);
    }

    private static List<JsonElement> Templates()
    {
        var resource = typeof(SystemTemplateSeedService).Assembly.GetManifestResourceNames().Single(x => x.EndsWith("system-templates.json", StringComparison.Ordinal));
        using var stream = typeof(SystemTemplateSeedService).Assembly.GetManifestResourceStream(resource)!;

        return [.. JsonDocument.Parse(stream).RootElement.Clone().EnumerateArray()];
    }
}
