using CodeDesignPlus.Net.Microservice.Emails.Domain.Enums;

namespace CodeDesignPlus.Net.Microservice.Emails.Domain;

public class UserAggregate(Guid id) : AggregateRootBase(id)
{
    public Guid IdTemplate { get; private set; }    
    public string Subject { get; private set; } = null!;
    public string UriLoginApp { get; private set; } = null!;
    public TypeTemplate Type { get; private set; }

    public UserAggregate(Guid id, Guid idTemplate, TypeTemplate typeTemplate, string subject, string uriLoginApp) : this(id)
    {
        DomainGuard.GuidIsEmpty(idTemplate, Errors.IdTemplateIsInvalid);
        DomainGuard.IsTrue(typeTemplate == TypeTemplate.None, Errors.TypeTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(subject, Errors.SubjectIsInvalid);
        DomainGuard.IsNullOrEmpty(uriLoginApp, Errors.UriLoginAppIsInvalid);

        IdTemplate = idTemplate;
        Type = typeTemplate;
        Subject = subject;
        UriLoginApp = uriLoginApp;
        CreatedAt = SystemClock.Instance.GetCurrentInstant();
    }

    public static UserAggregate Create(Guid id, TypeTemplate typeTemplate, string subject, string uriLoginApp)
    {
        DomainGuard.GuidIsEmpty(id, Errors.IdEmailIsInvalid);
        DomainGuard.IsTrue(typeTemplate == TypeTemplate.None, Errors.TypeTemplateIsInvalid);

        return new UserAggregate(id, Guid.Empty, typeTemplate, subject, uriLoginApp);
    }

    public void UpdateTemplate(Guid idTemplate, TypeTemplate typeTemplate, string subject, string uriLoginApp)
    {
        DomainGuard.GuidIsEmpty(idTemplate, Errors.IdTemplateIsInvalid);
        DomainGuard.IsTrue(typeTemplate == TypeTemplate.None, Errors.TypeTemplateIsInvalid);
        DomainGuard.IsNullOrEmpty(subject, Errors.SubjectIsInvalid);
        DomainGuard.IsNullOrEmpty(uriLoginApp, Errors.UriLoginAppIsInvalid);

        IdTemplate = idTemplate;
        Type = typeTemplate;
        Subject = subject;
        UriLoginApp = uriLoginApp;
        UpdatedAt = SystemClock.Instance.GetCurrentInstant();
    }
}
