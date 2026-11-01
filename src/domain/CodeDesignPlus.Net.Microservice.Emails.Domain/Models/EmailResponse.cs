namespace CodeDesignPlus.Net.Microservice.Emails.Domain.Models;

/// <summary>
/// The result of handing an email to the provider.
/// </summary>
/// <remarks>
/// The state is explicit: before, the caller inferred it from <see cref="Error"/> and the success branch passed the two
/// strings in the wrong order, so every accepted email was stored as failed (pendings/680).
/// </remarks>
public class EmailResponse
{
    /// <summary>
    /// Gets a value indicating whether the provider accepted the email.
    /// </summary>
    public bool Succeeded { get; private set; }
    /// <summary>
    /// Gets the error message, if any.
    /// </summary>
    public string? Error { get; private set; }
    /// <summary>
    /// Gets the status code of the email response.
    /// </summary>
    public string? StatusCode { get; private set; }

    /// <summary>
    /// Creates the response of an email the provider accepted.
    /// </summary>
    /// <param name="statusCode">The status code returned by the provider.</param>
    public static EmailResponse Success(string? statusCode)
    {
        return new EmailResponse
        {
            Succeeded = true,
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Creates the response of an email the provider rejected.
    /// </summary>
    /// <param name="error">The error message returned by the provider.</param>
    /// <param name="statusCode">The error code returned by the provider.</param>
    public static EmailResponse Failure(string? error, string? statusCode)
    {
        return new EmailResponse
        {
            Succeeded = false,
            Error = error,
            StatusCode = statusCode
        };
    }
}
