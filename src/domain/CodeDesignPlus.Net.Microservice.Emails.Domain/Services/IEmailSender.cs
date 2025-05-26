using CodeDesignPlus.Net.Microservice.Emails.Domain.Models;

namespace CodeDesignPlus.Net.Microservice.Emails.Domain.Services;

/// <summary>
///  Service to send emails
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email message
    /// </summary>
    /// <param name="emailMessage">Message to be sent</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return a Task that represents the asynchronous operation. The task result contains the email response.</returns>
    Task<EmailResponse> SendEmail(EmailMessage emailMessage, CancellationToken cancellationToken);
    /// <summary>
    /// Builds the body of the email message using a template and values
    /// </summary>
    /// <param name="template">
    /// <param name="values"></param>
    /// <returns></returns>
    string BuildBody(string template, Dictionary<string, string> values);


}
