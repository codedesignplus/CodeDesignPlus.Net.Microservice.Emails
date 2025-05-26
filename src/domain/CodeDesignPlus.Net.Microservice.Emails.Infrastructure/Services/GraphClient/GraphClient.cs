using Microsoft.Graph;
using Azure.Identity;
using CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Options;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Infrastructure.Services.GraphClient;

/// <summary>
/// Implementation of the IGraphClient interface for interacting with Microsoft Graph API.
/// </summary>
public class GraphClient : IGraphClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GraphClient"/> class.
    /// </summary>
    public GraphServiceClient Client { get; private set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphClient"/> class with the specified options.
    /// </summary>
    /// <param name="graphOptions">The options for configuring the Graph client.</param>
    public GraphClient(IOptions<EmailOptions> graphOptions)
    {
        ArgumentNullException.ThrowIfNull(graphOptions);

        var options = new ClientSecretCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
        };

        var clientSecretCredential = new ClientSecretCredential(
            graphOptions.Value.TenantId,
            graphOptions.Value.ClientId,
            graphOptions.Value.ClientSecret,
            options
        );

        var graphClient = new GraphServiceClient(clientSecretCredential, graphOptions.Value.Scopes);

        this.Client = graphClient;
    }
}
