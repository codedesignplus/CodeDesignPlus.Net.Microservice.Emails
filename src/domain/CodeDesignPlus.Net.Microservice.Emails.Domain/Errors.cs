using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Emails.Domain;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("100");

    public static readonly Error IdEmailIsInvalid = new("101"); 
    public static readonly Error BccEmailIsInvalid = new("102");
    public static readonly Error SubjectEmailIsInvalid = new("103"); 
    public static readonly Error BodyEmailIsInvalid = new("104"); 
    public static readonly Error FromEmailIsInvalid = new("105"); 
    public static readonly Error AttachmentsEmailIsInvalid = new("106"); 
    public static readonly Error ServerEmailIsInvalid = new("107");
    public static readonly Error CcEmailIsInvalid = new("108");
    public static readonly Error ToEmailIsInvalid = new("109");

    public static readonly Error IdTemplateIsInvalid = new("110");
    public static readonly Error NameTemplateIsInvalid = new("111");
    public static readonly Error SubjectTemplateIsInvalid = new("112");
    public static readonly Error BodyTemplateIsInvalid = new("113");
    public static readonly Error VariablesTemplateIsInvalid = new("114");
    public static readonly Error AttachmentsTemplateIsInvalid = new("115");

    public static readonly Error FromTemplateIsInvalid = new("116");
    public static readonly Error AliasTemplateIsInvalid = new("117");

    public static readonly Error TypeTemplateIsInvalid = new("118");

    public static readonly Error SubjectIsInvalid = new("119");

    public static readonly Error UriLoginAppIsInvalid = new("120");

    public static readonly Error FileAttachmentIdIsInvalid = new("121");
    public static readonly Error FileAttachmentNameIsInvalid = new("122");
    public static readonly Error FileAttachmentTargetIsInvalid = new("123");
}
