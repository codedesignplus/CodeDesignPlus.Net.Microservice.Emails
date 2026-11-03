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

    /// <summary>
    /// El correo y los recibos de una licencia no dicen «IVA (19%)» fijo: la fila del impuesto la manda ms-licenses según
    /// el tratamiento del precio (gravado, excluido o exento, pendings/662), y está declarada en la plantilla.
    /// </summary>
    [Theory]
    [InlineData("PurchaseConfirmation")]
    [InlineData("PurchaseReceipt")]
    [InlineData("SubscriptionReceipt")]
    public void Seed_LicenseReceipts_TakeTheTaxRowFromTheSender(string name)
    {
        // Arrange
        var template = Templates().Single(x => x.GetProperty("name").GetString() == name);

        // Act
        var body = template.GetProperty("body").GetString()!;
        var declared = template.GetProperty("variables").EnumerateArray().Any(x => x.GetString() == "tax_label");

        // Assert
        Assert.Equal((true, false, true), (body.Contains("{{tax_label}}", StringComparison.Ordinal), body.Contains(">IVA", StringComparison.Ordinal), declared));
    }

    /// <summary>
    /// Los importes de los correos y recibos de licencias llegan escritos con su símbolo («$ 773.500,00»): la plantilla no
    /// les pega el código de la moneda detrás, que daba «773,500.00 COP» (pendings/738).
    /// </summary>
    [Theory]
    [InlineData("PurchaseConfirmation")]
    [InlineData("PurchaseReceipt")]
    [InlineData("SubscriptionRenewed")]
    [InlineData("SubscriptionReceipt")]
    public void Seed_LicenseReceipts_DoNotAppendTheCurrencyCodeToTheAmounts(string name)
    {
        // Arrange
        var template = Templates().Single(x => x.GetProperty("name").GetString() == name);

        // Act
        var body = template.GetProperty("body").GetString()!;

        // Assert
        Assert.DoesNotContain("{{currency}}", body, StringComparison.Ordinal);
    }

    /// <summary>
    /// Sin crédito la fila «Crédito aplicado» no se ve: ms-licenses manda <c>none</c> y la plantilla, que solo reemplaza
    /// variables, la oculta con ese valor (pendings/738).
    /// </summary>
    [Fact]
    public void Seed_SubscriptionReceipt_TheCreditRowCanBeHidden()
    {
        // Arrange
        var body = Templates().Single(x => x.GetProperty("name").GetString() == "SubscriptionReceipt").GetProperty("body").GetString()!;

        // Act
        var row = body[body.LastIndexOf("<tr", body.IndexOf("Crédito aplicado", StringComparison.Ordinal), StringComparison.Ordinal)..body.IndexOf("Crédito aplicado", StringComparison.Ordinal)];

        // Assert
        Assert.Contains("display: {{credit_row_display}};", row, StringComparison.Ordinal);
    }

    /// <summary>
    /// El recibo y el correo de una renovación dicen qué periodo se pagó (pendings/738), y la variable está declarada.
    /// </summary>
    [Theory]
    [InlineData("SubscriptionRenewed")]
    [InlineData("SubscriptionReceipt")]
    public void Seed_SubscriptionTemplates_SayThePaidPeriod(string name)
    {
        // Arrange
        var template = Templates().Single(x => x.GetProperty("name").GetString() == name);

        // Act
        var body = template.GetProperty("body").GetString()!;
        var declared = template.GetProperty("variables").EnumerateArray().Any(x => x.GetString() == "period");

        // Assert
        Assert.Equal((true, true), (body.Contains("{{period}}", StringComparison.Ordinal), declared));
    }

    private static List<JsonElement> Templates()
    {
        var resource = typeof(SystemTemplateSeedService).Assembly.GetManifestResourceNames().Single(x => x.EndsWith("system-templates.json", StringComparison.Ordinal));
        using var stream = typeof(SystemTemplateSeedService).Assembly.GetManifestResourceStream(resource)!;

        return [.. JsonDocument.Parse(stream).RootElement.Clone().EnumerateArray()];
    }
}
