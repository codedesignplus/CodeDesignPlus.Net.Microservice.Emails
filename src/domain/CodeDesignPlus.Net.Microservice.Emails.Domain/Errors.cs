using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Emails.Domain;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("100", "UnknownError");

    public static readonly Error IdEmailIsInvalid = new("101", "The id is invalid"); 
    public static readonly Error BccEmailIsInvalid = new("102", "The copy hidden email is invalid");
    public static readonly Error SubjectEmailIsInvalid = new("103", "The Subject is invalid"); 
    public static readonly Error BodyEmailIsInvalid = new("104", "The Body is invalid"); 
    public static readonly Error FromEmailIsInvalid = new("105", "The From is invalid"); 
    public static readonly Error AttachmentsEmailIsInvalid = new("106", "The Attachments are invalid"); 
    public static readonly Error ServerEmailIsInvalid = new("107", "The Server is invalid");
    public static readonly Error CcEmailIsInvalid = new("108", "The copy email is invalid");
    public static readonly Error ToEmailIsInvalid = new("109", "The destination email is invalid");

    public static readonly Error IdTemplateIsInvalid = new("110", "The id template is invalid");
    public static readonly Error NameTemplateIsInvalid = new("111", "The name template is invalid");
    public static readonly Error SubjectTemplateIsInvalid = new("112", "The subject template is invalid");
    public static readonly Error BodyTemplateIsInvalid = new("113", "The body template is invalid");
    public static readonly Error VariablesTemplateIsInvalid = new("114", "The variables template is invalid");
    public static readonly Error AttachmentsTemplateIsInvalid = new("115", "The attachments template is invalid");

    public static readonly Error FromTemplateIsInvalid = new("116", "The from template is invalid");
    public static readonly Error AliasTemplateIsInvalid = new("117", "The alias template is invalid");

    public static readonly Error TypeTemplateIsInvalid = new("118", "The type template is invalid");

    public static readonly Error SubjectIsInvalid = new("119", "The subject is invalid");

    public static readonly Error UriLoginAppIsInvalid = new("120", "The URI of the login application is invalid");

    public static readonly Error FileAttachmentIdIsInvalid = new("121", "The file attachment id is invalid");
    public static readonly Error FileAttachmentNameIsInvalid = new("122", "The file attachment name is invalid");
    public static readonly Error FileAttachmentTargetIsInvalid = new("123", "The file attachment target is invalid");
}
