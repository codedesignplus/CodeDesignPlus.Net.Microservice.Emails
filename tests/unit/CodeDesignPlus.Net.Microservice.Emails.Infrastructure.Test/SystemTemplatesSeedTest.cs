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
    [InlineData("SubscriptionChanged")]
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
    [InlineData("SubscriptionChanged")]
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

    /// <summary>
    /// El correo de un cambio de licencia es suyo (pendings/738): es un tipo de plantilla, dice el precio, el crédito
    /// aplicado y lo pagado, y declara todas sus variables.
    /// </summary>
    [Fact]
    public void Seed_SubscriptionChanged_ShowsPriceCreditAppliedAndAmountPaid()
    {
        // Arrange
        var template = Templates().Single(x => x.GetProperty("name").GetString() == "SubscriptionChanged");
        var body = template.GetProperty("body").GetString()!;
        var declared = template.GetProperty("variables").EnumerateArray().Select(x => x.GetString()).ToHashSet();

        // Act
        var undeclared = System.Text.RegularExpressions.Regex.Matches(body, @"\{\{(\w+)\}\}").Select(match => match.Groups[1].Value).Where(name => !declared.Contains(name)).ToList();

        // Assert
        Assert.Equal((true, true, 0), (Enum.TryParse<TypeTemplate>("SubscriptionChanged", out _), new[] { "{{price}}", "{{credit_applied}}", "{{amount_paid}}" }.All(x => body.Contains(x, StringComparison.Ordinal)), undeclared.Count));
    }

    /// <summary>
    /// El correo del cambio lleva el comprobante adjunto: no promete otro correo, como hacía la confirmación de compra que
    /// usaba antes (pendings/738).
    /// </summary>
    [Fact]
    public void Seed_SubscriptionChanged_DoesNotPromiseASeparateEmail()
    {
        // Arrange
        var body = Templates().Single(x => x.GetProperty("name").GetString() == "SubscriptionChanged").GetProperty("body").GetString()!;

        // Act
        var promises = body.Contains("correo separado", StringComparison.OrdinalIgnoreCase) || body.Contains("correo aparte", StringComparison.OrdinalIgnoreCase);

        // Assert
        Assert.False(promises);
    }

    /// <summary>
    /// En el PDF del comprobante los importes no se parten en dos líneas («-$» arriba y «290.000,00» abajo): cada celda de
    /// importe lleva <c>white-space: nowrap</c> (pendings/738).
    /// </summary>
    [Fact]
    public void Seed_SubscriptionReceipt_AmountsDoNotWrap()
    {
        // Arrange
        var body = Templates().Single(x => x.GetProperty("name").GetString() == "SubscriptionReceipt").GetProperty("body").GetString()!;

        // Act
        var cells = System.Text.RegularExpressions.Regex.Matches(body, @"<td style=""[^""]*text-align: right;[^""]*"">\{\{(credit_applied|amount_paid|price|tax|subtotal)\}\}|<td style=""[^""]*text-align: right;[^""]*"">-\{\{credit_applied\}\}").Select(x => x.Value).ToList();

        // Assert
        Assert.True(cells.Count == 5 && cells.All(x => x.Contains("white-space: nowrap", StringComparison.Ordinal)));
    }

    /// <summary>
    /// Los comprobantes y correos de licencia muestran los paquetes complementarios de la orden y declaran sus variables
    /// (pendings/739).
    /// </summary>
    [Theory]
    [InlineData("PurchaseConfirmation")]
    [InlineData("PurchaseReceipt")]
    [InlineData("SubscriptionRenewed")]
    [InlineData("SubscriptionReceipt")]
    [InlineData("SubscriptionChanged")]
    public void Seed_LicenseTemplates_ShowThePackages(string name)
    {
        // Arrange
        var template = Templates().Single(x => x.GetProperty("name").GetString() == name);
        var body = template.GetProperty("body").GetString()!;
        var declared = template.GetProperty("variables").EnumerateArray().Select(x => x.GetString()).ToHashSet();

        // Act
        var undeclared = System.Text.RegularExpressions.Regex.Matches(body, @"\{\{(\w+)\}\}").Select(match => match.Groups[1].Value).Where(x => !declared.Contains(x)).ToList();

        // Assert
        Assert.Equal((true, true, 0), (body.Contains("{{packages_list}}", StringComparison.Ordinal), body.Contains("display: {{packages_row_display}};", StringComparison.Ordinal), undeclared.Count));
    }

    /// <summary>
    /// La compra o la ampliación de un paquete tiene su correo (pendings/739): es un tipo de plantilla, dice el precio, el
    /// crédito aplicado y lo pagado, y declara sus variables.
    /// </summary>
    [Fact]
    public void Seed_SubscriptionPackagePurchased_ShowsPriceCreditAppliedAndAmountPaid()
    {
        // Arrange
        var template = Templates().Single(x => x.GetProperty("name").GetString() == "SubscriptionPackagePurchased");
        var body = template.GetProperty("body").GetString()!;
        var declared = template.GetProperty("variables").EnumerateArray().Select(x => x.GetString()).ToHashSet();

        // Act
        var undeclared = System.Text.RegularExpressions.Regex.Matches(body, @"\{\{(\w+)\}\}").Select(match => match.Groups[1].Value).Where(x => !declared.Contains(x)).ToList();

        // Assert
        Assert.Equal((17, true, 0), ((int)Enum.Parse<TypeTemplate>("SubscriptionPackagePurchased"), new[] { "{{price}}", "{{credit_applied}}", "{{amount_paid}}" }.All(x => body.Contains(x, StringComparison.Ordinal)), undeclared.Count));
    }

    /// <summary>
    /// En el comprobante de una compra de paquete no salen la licencia ni su impuesto, que no se cobran (pendings/739).
    /// </summary>
    [Fact]
    public void Seed_SubscriptionReceipt_TheLicenseRowsCanBeHidden()
    {
        // Arrange
        var body = Templates().Single(x => x.GetProperty("name").GetString() == "SubscriptionReceipt").GetProperty("body").GetString()!;

        // Act
        var hidden = System.Text.RegularExpressions.Regex.Matches(body, @"display: \{\{license_row_display\}\};").Count;

        // Assert
        Assert.Equal(2, hidden);
    }

    /// <summary>
    /// The receipt of a package purchase is its own template (pendings/739): ms-licenses asks for it by name, so it must be a
    /// template type; it breaks the package down into base, tax and total, says how long it lasts and declares every
    /// variable it uses.
    /// </summary>
    [Fact]
    public void Seed_PackageReceipt_ShowsBaseTaxTotalAndTerm()
    {
        // Arrange
        var template = Templates().Single(x => x.GetProperty("name").GetString() == "PackageReceipt");
        var body = template.GetProperty("body").GetString()!;
        var declared = template.GetProperty("variables").EnumerateArray().Select(x => x.GetString()).ToHashSet();

        // Act
        var undeclared = System.Text.RegularExpressions.Regex.Matches(body, @"\{\{(\w+)\}\}").Select(match => match.Groups[1].Value).Where(x => !declared.Contains(x)).ToList();

        // Assert
        Assert.Equal((18, true, 0), ((int)Enum.Parse<TypeTemplate>("PackageReceipt"), new[] { "{{package_subtotal}}", "{{package_tax_label}}", "{{package_tax}}", "{{package_total}}", "{{package_period}}", "{{amount_paid}}" }.All(x => body.Contains(x, StringComparison.Ordinal)), undeclared.Count));
    }

    /// <summary>
    /// The package receipt does not list the modules of the license: they are not part of what was bought (pendings/739).
    /// </summary>
    [Fact]
    public void Seed_PackageReceipt_DoesNotListTheLicenseModules()
    {
        // Arrange
        var body = Templates().Single(x => x.GetProperty("name").GetString() == "PackageReceipt").GetProperty("body").GetString()!;

        // Act
        var mentionsModules = body.Contains("{{modules_list}}", StringComparison.Ordinal) || body.Contains("Módulos", StringComparison.Ordinal);

        // Assert
        Assert.False(mentionsModules);
    }

    private static List<JsonElement> Templates()
    {
        var resource = typeof(SystemTemplateSeedService).Assembly.GetManifestResourceNames().Single(x => x.EndsWith("system-templates.json", StringComparison.Ordinal));
        using var stream = typeof(SystemTemplateSeedService).Assembly.GetManifestResourceStream(resource)!;

        return [.. JsonDocument.Parse(stream).RootElement.Clone().EnumerateArray()];
    }
}
