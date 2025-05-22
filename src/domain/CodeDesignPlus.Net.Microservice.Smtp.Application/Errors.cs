namespace CodeDesignPlus.Net.Microservice.Smtp.Application;

public class Errors: IErrorCodes
{    
    public const string UnknownError = "200 : UnknownError";

    public const string InvalidRequest = "201 : Invalid Request";
    public const string EmailsAlreadyWasSent = "202 : Emails already was sent"; 
    public const string TemplateAlreadyExists = "203 : Template already exists"; 
    public const string TeemplateNotFound = "204 : Template not found"; 
}
