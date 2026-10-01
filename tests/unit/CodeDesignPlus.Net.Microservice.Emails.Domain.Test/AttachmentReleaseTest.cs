using System.Collections.Generic;
using System.Linq;
using CodeDesignPlus.Net.Core.Abstractions.Contracts;
using CodeDesignPlus.Net.Microservice.Emails.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Domain.Test;

/// <summary>
/// Los adjuntos que una plantilla deja de usar se le avisan a ms-filestorage, en el espacio que les corresponde
/// (pendings/172).
/// </summary>
public class AttachmentReleaseTest
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly FileAttachment Terms = new(Guid.NewGuid(), "terminos.pdf", "email-templates");
    private static readonly FileAttachment Rules = new(Guid.NewGuid(), "reglamento.pdf", "email-templates");

    private static TemplateAggregate Template(Guid? tenant)
    {
        var template = TemplateAggregate.Create(Guid.NewGuid(), "welcome", "Bienvenido", "<p>Hola</p>", [], [Terms, Rules], "no-reply@kappali.com", "Kappali", true, tenant, UserId);
        template.GetAndClearEvents();

        return template;
    }

    private static void UpdateAttachments(TemplateAggregate template, List<FileAttachment> attachments) =>
        template.Update(template.Name, template.Subject, template.Body, template.Variables, attachments, template.From, template.Alias, template.IsHtml, UserId);

    private static FilesReleasedDomainEvent? Released(TemplateAggregate template) =>
        template.GetAndClearEvents().OfType<FilesReleasedDomainEvent>().SingleOrDefault();

    [Fact]
    public void Update_AttachmentRemoved_ReleasesItInTheTenant()
    {
        var tenant = Guid.NewGuid();
        var template = Template(tenant);

        UpdateAttachments(template, [Terms]);

        var released = Released(template);
        Assert.NotNull(released);
        Assert.Equal([Rules.Id], released.Files);
        Assert.Equal(tenant, released.Tenant);
    }

    [Fact]
    public void Update_SameAttachments_ReleasesNothing()
    {
        var template = Template(Guid.NewGuid());

        UpdateAttachments(template, [Terms, Rules]);

        Assert.Null(Released(template));
    }

    [Fact]
    public void Delete_SystemTemplate_ReleasesAllInThePlatform()
    {
        var template = Template(null);

        template.Delete(UserId);

        var released = Released(template);
        Assert.NotNull(released);
        Assert.Equal([Terms.Id, Rules.Id], released.Files);
        Assert.Equal(Guid.Empty, released.Tenant);
    }
}
