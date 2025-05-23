using Microsoft.Graph;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;

/// <summary>
/// Interface for GraphClient service.
/// </summary>
public interface IGraphClient
{
    /// <summary>
    /// Gets the GraphServiceClient instance.
    /// </summary>
    GraphServiceClient Client { get; }
}
