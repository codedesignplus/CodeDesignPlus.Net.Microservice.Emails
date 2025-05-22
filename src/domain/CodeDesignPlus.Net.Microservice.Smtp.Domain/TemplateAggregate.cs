namespace CodeDesignPlus.Net.Microservice.Smtp.Domain;

public class TemplateAggregate(Guid id) : AggregateRootBase(id)
{
    public string Name { get; private set; } = null!;
    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public List<string> Variables { get; private set; } = [];
    public List<string> Attachments { get; private set; } = [];
    public Guid? Tenant { get; private set; } = null!;

    public TemplateAggregate(Guid id, string name, string subject, string body, List<string> variables, List<string> attachments, Guid? tenant, Guid createdBy) : this(id)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(name, Errors.NameTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(subject, Errors.SubjectTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(body, Errors.BodyTemplateIsInvalid);
        DomainGuard.IsEmpty(variables, Errors.VariablesTemplateIsInvalid);
        DomainGuard.IsEmpty(attachments, Errors.AttachmentsTemplateIsInvalid);

        Name = name;
        Subject = subject;
        Body = body;
        Variables = variables;
        Attachments = attachments;
        Tenant = tenant;

        CreatedAt = SystemClock.Instance.GetCurrentInstant();
        CreatedBy = createdBy;

        this.AddEvent(TemplateCreatedDomainEvent.Create(Id, Name, Subject, Body, Variables, Attachments, Tenant));
    }

    public static TemplateAggregate Create(Guid id, string name, string subject, string body, List<string> variables, List<string> attachments, Guid? tenant, Guid createdBy)
    {
        return new TemplateAggregate(id, name, subject, body, variables, attachments, tenant, createdBy);
    }

    public void Update(string name, string subject, string body, List<string> variables, List<string> attachments, Guid updatedBy)
    {
        DomainGuard.IsNullOrEmpty(name, Errors.NameTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(subject, Errors.SubjectTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(body, Errors.BodyTemplateIsInvalid);
        DomainGuard.IsEmpty(variables, Errors.VariablesTemplateIsInvalid);
        DomainGuard.IsEmpty(attachments, Errors.AttachmentsTemplateIsInvalid);

        Name = name;
        Subject = subject;
        Body = body;
        Variables = variables;
        Attachments = attachments;

        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        UpdatedBy = updatedBy;

        this.AddEvent(TemplateUpdatedDomainEvent.Create(Id, Name, Subject, Body, Variables, Attachments, Tenant));
    }

    public void Delete(Guid updatedBy)
    {
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
        UpdatedBy = updatedBy;

        this.AddEvent(TemplateDeletedDomainEvent.Create(Id, Name, Subject, Body, Variables, Attachments, Tenant));
    }
    

}