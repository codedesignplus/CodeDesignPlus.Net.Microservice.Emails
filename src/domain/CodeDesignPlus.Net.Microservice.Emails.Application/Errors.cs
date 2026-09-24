using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Emails.Application;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("200");

    public static readonly Error InvalidRequest = new("201");
    public static readonly Error EmailsAlreadyWasSent = new("202"); 
    public static readonly Error TemplateAlreadyExists = new("203"); 
    public static readonly Error TemplateNotFound = new("204");
    public static readonly Error TemplatePasswordTempNotFound = new("205");
    public static readonly Error SecretContextNotFound = new("206");

    public static readonly Error EmailNotFound = new("207");

    public static readonly Error UserConfigTemplateAlreadyExists = new("208");
    public static readonly Error UserConfigTemplateNotFound = new("209");

    public static readonly Error TemplateInvitationToOrganizationNotFound = new("210");
}
