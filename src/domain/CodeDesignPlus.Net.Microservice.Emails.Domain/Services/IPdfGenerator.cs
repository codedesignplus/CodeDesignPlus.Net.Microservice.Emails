namespace CodeDesignPlus.Net.Microservice.Emails.Domain.Services;

public interface IPdfGenerator
{
    Task<byte[]> GenerateFromHtmlAsync(string html, CancellationToken cancellationToken);
}
