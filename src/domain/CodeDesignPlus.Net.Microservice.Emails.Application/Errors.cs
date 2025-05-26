namespace CodeDesignPlus.Net.Microservice.Emails.Application;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "200 : UnknownError";

    public const string InvalidRequest = "201 : Invalid Request";
    public const string EmailsAlreadyWasSent = "202 : Emails already was sent"; 
    public const string TemplateAlreadyExists = "203 : Template already exists"; 
    public const string TemplateNotFound = "204 : Template not found";
    public const string TemplatePasswordTempNotFound = "205 : Template Password Temp not found";
    public const string SecretContextNotFound = "206 : Secret context is not valid or not found in Vault options.";

    public const string EmailNotFound = "207 : Email not found";
}
