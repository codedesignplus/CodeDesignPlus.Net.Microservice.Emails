using System.Text.Json;
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
}
