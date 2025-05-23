namespace CodeDesignPlus.Net.Microservice.Smtp.Domain.Models;

/// <summary>
/// Represents an email message.
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Gets or sets the list of recipients for the email message.
    /// </summary>
    public List<string> To { get; private set; } = [];
    /// <summary>
    /// Gets or sets the list of carbon copy (CC) recipients for the email message.
    /// </summary>
    public List<string> Cc { get; private set; } = [];
    /// <summary>
    /// Gets or sets the list of blind carbon copy (BCC) recipients for the email message.
    /// </summary>
    public List<string> Bcc { get; private set; } = [];
    /// <summary>
    /// Gets or sets the subject of the email message.
    /// </summary>
    public string Subject { get; private set; } = null!;
    /// <summary>
    /// Gets or sets the body of the email message.
    /// </summary>
    public string Body { get; private set; } = null!;
    /// <summary>
    /// Gets or sets the sender's email address.
    /// </summary>
    public string From { get; private set; } = null!;
    /// <summary>
    /// Gets or sets the sender's alias.
    /// </summary>
    public string Alias { get; private set; } = null!;
    /// <summary>
    /// Gets or sets the list of attachments for the email message.
    /// </summary>
    public List<string> Attachments { get; private set; } = [];
    /// <summary>
    /// Gets or sets a value indicating whether the email message is in HTML format.
    /// </summary>
    public bool IsHtm { get; private set; } = true;

    /// <summary>
    /// Creates a new instance of the <see cref="EmailMessage"/> class.
    /// </summary>
    /// <param name="subject">The subject of the email message.</param>
    /// <param name="body">The body of the email message.</param>
    /// <param name="to">The list of recipients for the email message.</param>
    /// <param name="cc">The list of carbon copy (CC) recipients for the email message.</param>
    /// <param name="bcc">The list of blind carbon copy (BCC) recipients for the email message.</param>
    /// <param name="from">The sender's email address.</param>
    /// <param name="alias">The sender's alias.</param>
    /// <param name="attachments">The list of attachments for the email message.</param>
    /// <param name="isHtm">A value indicating whether the email message is in HTML format.</param>
    /// <returns>A new instance of the <see cref="EmailMessage"/> class.</returns>
    public static EmailMessage Create(string subject, string body, List<string> to, List<string> cc, List<string> bcc, string from, string alias, List<string> attachments, bool isHtm)
    {
        return new EmailMessage
        {
            Subject = subject,
            Body = body,
            To = to,
            Cc = cc,
            Bcc = bcc,
            From = from,
            Alias = alias,
            Attachments = attachments,
            IsHtm = isHtm
        };
    }
}
