using CodeDesignPlus.Net.Exceptions;

namespace CodeDesignPlus.Net.Microservice.Emails.Application;

public class Errors: IErrorCodes
{    
    public static readonly Error UnknownError = new("200", "UnknownError");

    public static readonly Error InvalidRequest = new("201", "Invalid Request");
    public static readonly Error EmailsAlreadyWasSent = new("202", "Emails already was sent"); 
    public static readonly Error TemplateAlreadyExists = new("203", "Template already exists"); 
    public static readonly Error TemplateNotFound = new("204", "Template not found");
    public static readonly Error TemplatePasswordTempNotFound = new("205", "Template Password Temp not found");
    public static readonly Error SecretContextNotFound = new("206", "Secret context is not valid or not found in Vault options.");

    public static readonly Error EmailNotFound = new("207", "Email not found");

    public static readonly Error UserConfigTemplateAlreadyExists = new("208", "User config template already exists");
    public static readonly Error UserConfigTemplateNotFound = new("209", "User config template not found");

    public static readonly Error TemplateInvitationToOrganizationNotFound = new("210", "Template InvitationToOrganization not found");
}
