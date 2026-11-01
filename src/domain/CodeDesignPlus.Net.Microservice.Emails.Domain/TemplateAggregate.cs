using System.Buffers.Text;
using System.Text;
using CodeDesignPlus.Net.Microservice.Emails.Domain.ValueObjects;

namespace CodeDesignPlus.Net.Microservice.Emails.Domain;

public class TemplateAggregate(Guid id) : AggregateRootBase(id)
{
    public string Name { get; private set; } = null!;
    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string From { get; private set; } = null!;
    public string Alias { get; private set; } = null!;
    public bool IsHtml { get; private set; }
    public List<string> Variables { get; private set; } = [];
    /// <summary>
    /// Las variables cuyo valor no se guarda en el historial: el correo sale con el valor real y lo guardado lo lleva
    /// enmascarado (pendings/680).
    /// </summary>
    public List<string> SensitiveVariables { get; private set; } = [];
    public List<FileAttachment> Attachments { get; private set; } = [];
    public Guid? Tenant { get; private set; } = null!;

    public TemplateAggregate(Guid id, string name, string subject, string body, List<string> variables, List<FileAttachment> attachments, string from, string alias, bool isHtml, Guid? tenant, Guid createdBy, List<string>? sensitiveVariables = null) : this(id)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(name, Errors.NameTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(subject, Errors.SubjectTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(body, Errors.BodyTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(from, Errors.FromTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(alias, Errors.AliasTemplateIsInvalid);

        Name = name;
        Subject = subject;
        Body = Base64.IsValid(body) ? body : Convert.ToBase64String(Encoding.UTF8.GetBytes(body));
        Variables = variables ?? [];
        SensitiveVariables = ValidSensitiveVariables(sensitiveVariables, Variables);
        Attachments = attachments ?? [];
        Tenant = tenant == Guid.Empty ? null : tenant;
        From = from;
        Alias = alias;
        IsHtml = isHtml;
        IsActive = true;

        CreatedAt = SystemClock.Instance.GetCurrentInstant();
        CreatedBy = createdBy;

        this.AddEvent(TemplateCreatedDomainEvent.Create(Id, Name, Subject, Body, Variables, Attachments, Tenant));
    }

    public static TemplateAggregate Create(Guid id, string name, string subject, string body, List<string> variables, List<FileAttachment> attachments, string from, string alias, bool isHtml, Guid? tenant, Guid createdBy, List<string>? sensitiveVariables = null)
    {
        return new TemplateAggregate(id, name, subject, body, variables, attachments, from, alias, isHtml, tenant, createdBy, sensitiveVariables);
    }

    /// <summary>
    /// Actualiza la plantilla. Sin <paramref name="sensitiveVariables"/> se conservan las que tenía, siempre que sigan
    /// entre las variables.
    /// </summary>
    public void Update(string name, string subject, string body, List<string> variables, List<FileAttachment> attachments, string from, string alias, bool isHtml, Guid updatedBy, List<string>? sensitiveVariables = null)
    {
        DomainGuard.IsNullOrEmpty(name, Errors.NameTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(subject, Errors.SubjectTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(body, Errors.BodyTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(from, Errors.FromTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(alias, Errors.AliasTemplateIsInvalid);

        Name = name;
        Subject = subject;
        Body = Base64.IsValid(body) ? body : Convert.ToBase64String(Encoding.UTF8.GetBytes(body));
        Variables = variables ?? [];
        SensitiveVariables = ValidSensitiveVariables(sensitiveVariables ?? [.. SensitiveVariables.Where(Variables.Contains)], Variables);

        var removed = Attachments.Select(attachment => attachment.Id).Except((attachments ?? []).Select(attachment => attachment.Id)).ToList();

        Attachments = attachments ?? [];
        From = from;
        Alias = alias;
        IsHtml = isHtml;

        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        UpdatedBy = updatedBy;

        this.AddEvent(TemplateUpdatedDomainEvent.Create(Id, Name, Subject, Body, Variables, Attachments, Tenant));

        ReleaseFiles(removed, updatedBy);
    }

    /// <summary>
    /// Declara las variables sensibles de una plantilla ya guardada; lo usa la siembra para las plantillas del sistema
    /// y sus copias por copropiedad (pendings/680).
    /// </summary>
    public void DeclareSensitiveVariables(List<string> sensitiveVariables, Guid updatedBy)
    {
        SensitiveVariables = ValidSensitiveVariables(sensitiveVariables, Variables);

        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        UpdatedBy = updatedBy;
    }

    private static List<string> ValidSensitiveVariables(List<string>? sensitiveVariables, List<string> variables)
    {
        var sensitive = (sensitiveVariables ?? []).Distinct().ToList();

        DomainGuard.IsTrue(sensitive.Any(name => !variables.Contains(name)), Errors.SensitiveVariablesTemplateIsInvalid);

        return sensitive;
    }

    public void Delete(Guid deletedBy)
    {
        this.IsDeleted = true;
        this.IsActive = false;
        this.DeletedAt = SystemClock.Instance.GetCurrentInstant();
        this.DeletedBy = deletedBy;

        this.AddEvent(TemplateDeletedDomainEvent.Create(Id, Name, Subject, Body, Variables, Attachments, Tenant));

        ReleaseFiles(Attachments.Select(attachment => attachment.Id), deletedBy);
    }

    /// <summary>
    /// Avisa a ms-filestorage de los adjuntos que la plantilla ya no usa, para que los desactive (pendings/172). Los de
    /// una plantilla del sistema son de la plataforma (<see cref="Guid.Empty"/>, pendings/168).
    /// </summary>
    private void ReleaseFiles(IEnumerable<Guid> files, Guid releasedBy)
    {
        var released = files.ToList();

        if (released.Count > 0)
            this.AddEvent(FilesReleasedDomainEvent.Create(Id, released, releasedBy, Tenant ?? Guid.Empty));
    }
}