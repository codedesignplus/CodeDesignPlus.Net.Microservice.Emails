namespace CodeDesignPlus.Net.Microservice.Smtp.Domain;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "100 : UnknownError";

    public const string IdEmailIsInvalid = "101 : The id is invalid"; 
    public const string BccEmailIsInvalid = "102 : The copy hidden email is invalid";
    public const string SubjectEmailIsInvalid = "103 : The Subject is invalid"; 
    public const string BodyEmailIsInvalid = "104 : The Body is invalid"; 
    public const string FromEmailIsInvalid = "105 : The From is invalid"; 
    public const string AttachmentsEmailIsInvalid = "106 : The Attachments are invalid"; 
    public const string ServerEmailIsInvalid = "107 : The Server is invalid";
    public const string CcEmailIsInvalid = "108 : The copy email is invalid";
    public const string ToEmailIsInvalid = "109 : The destination email is invalid";

    public const string IdTemplateIsInvalid = "110 : The id template is invalid";
    public const string NameTemplateIsInvalid = "111 : The name template is invalid";
    public const string SubjectTemplateIsInvalid = "112 : The subject template is invalid";
    public const string BodyTemplateIsInvalid = "113 : The body template is invalid";
    public const string VariablesTemplateIsInvalid = "114 : The variables template is invalid";
    public const string AttachmentsTemplateIsInvalid = "115 : The attachments template is invalid";

    public const string FromTemplateIsInvalid = "116 : The from template is invalid";
    public const string AliasTemplateIsInvalid = "117 : The alias template is invalid"; 
}
