using System.Buffers.Text;
using System.Text;

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
    public List<string> Attachments { get; private set; } = [];
    public Guid? Tenant { get; private set; } = null!;

    public TemplateAggregate(Guid id, string name, string subject, string body, List<string> variables, List<string> attachments, string from, string alias, bool isHtml, Guid? tenant, Guid createdBy) : this(id)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(name, Errors.NameTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(subject, Errors.SubjectTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(body, Errors.BodyTemplateIsInvalid);
        DomainGuard.IsEmpty(variables, Errors.VariablesTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(from, Errors.FromTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(alias, Errors.AliasTemplateIsInvalid);

        Name = name;
        Subject = subject;
        Body = Base64.IsValid(body) ? body : Convert.ToBase64String(Encoding.UTF8.GetBytes(body));
        Variables = variables;
        Attachments = attachments;
        Tenant = tenant == Guid.Empty ? null : tenant;
        From = from;
        Alias = alias;
        IsHtml = isHtml;

        CreatedAt = SystemClock.Instance.GetCurrentInstant();
        CreatedBy = createdBy;

        this.AddEvent(TemplateCreatedDomainEvent.Create(Id, Name, Subject, Body, Variables, Attachments, Tenant));
    }

    public static TemplateAggregate Create(Guid id, string name, string subject, string body, List<string> variables, List<string> attachments, string from, string alias, bool isHtml, Guid? tenant, Guid createdBy)
    {
        return new TemplateAggregate(id, name, subject, body, variables, attachments, from, alias, isHtml, tenant, createdBy);
    }

    public void Update(string name, string subject, string body, List<string> variables, List<string> attachments, string from, string alias, bool isHtml, Guid updatedBy)
    {
        DomainGuard.IsNullOrEmpty(name, Errors.NameTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(subject, Errors.SubjectTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(body, Errors.BodyTemplateIsInvalid);
        DomainGuard.IsEmpty(variables, Errors.VariablesTemplateIsInvalid);
        DomainGuard.IsEmpty(attachments, Errors.AttachmentsTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(from, Errors.FromTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(alias, Errors.AliasTemplateIsInvalid);

        Name = name;
        Subject = subject;
        Body =  Base64.IsValid(body) ? body : Convert.ToBase64String(Encoding.UTF8.GetBytes(body));
        Variables = variables;
        Attachments = attachments;
        From = from;
        Alias = alias;
        IsHtml = isHtml;

        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        UpdatedBy = updatedBy;

        this.AddEvent(TemplateUpdatedDomainEvent.Create(Id, Name, Subject, Body, Variables, Attachments, Tenant));
    }

    public void Delete(Guid deletedBy)
    {
        this.IsDeleted = true;
        this.IsActive = false;
        this.DeletedAt = SystemClock.Instance.GetCurrentInstant();
        this.DeletedBy = deletedBy;

        this.AddEvent(TemplateDeletedDomainEvent.Create(Id, Name, Subject, Body, Variables, Attachments, Tenant));
    }


}