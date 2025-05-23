namespace CodeDesignPlus.Net.Microservice.Smtp.Domain.Models;

/// <summary>
/// Represents an email message.
/// </summary>
public class EmailResponse
{
    /// <summary>
    /// Gets or sets the error message, if any.
    /// </summary>
    public string? Error { get; private set; }
    /// <summary>
    /// Gets or sets the status code of the email response.
    /// </summary>
    public string? StatusCode { get; private set; }

    /// <summary>
    /// Creates a new instance of the <see cref="EmailResponse"/> class.
    /// </summary>
    /// <param name="error">The error message, if any.</param>
    /// <param name="statusCode">The status code of the email response.</param>
    /// <returns>A new instance of the <see cref="EmailResponse"/> class.</returns>
    public static EmailResponse Create(string? error, string? statusCode)
    {
        return new EmailResponse
        {
            Error = error,
            StatusCode = statusCode
        };
    }
}
