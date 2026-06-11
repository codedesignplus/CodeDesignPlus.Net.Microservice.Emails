using System.Reflection;
using System.Text.Json;
using CodeDesignPlus.Net.Microservice.Emails.Domain;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeDesignPlus.Net.Microservice.Emails.Infrastructure.Seeds;

public class SystemTemplateSeedService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<SystemTemplateSeedService> logger) : BackgroundService
{
    private static readonly Guid SystemUserId = Guid.Parse("10000000-0000-0000-0000-000000000001");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        logger.LogInformation("Starting system templates seed...");

        var templates = LoadResource<List<SystemTemplateSeedData>>("system-templates.json");

        if (templates == null || templates.Count == 0)
        {
            logger.LogWarning("No system templates found in embedded resource.");
            return;
        }

        using var scope = serviceScopeFactory.CreateScope();
        var templateRepository = scope.ServiceProvider.GetRequiredService<ITemplateRepository>();

        var seeded = 0;

        foreach (var data in templates)
        {
            try
            {
                var exists = await templateRepository.ExistsByNameAndTenantAsync(data.Name, null, stoppingToken);

                if (exists)
                    continue;

                var template = TemplateAggregate.Create(
                    Guid.NewGuid(),
                    data.Name,
                    data.Subject,
                    data.Body,
                    data.Variables,
                    [],
                    data.From,
                    data.Alias,
                    data.IsHtml,
                    null,
                    SystemUserId
                );

                await templateRepository.CreateAsync(template, stoppingToken);
                seeded++;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to seed template: {Name}", data.Name);
            }
        }

        logger.LogInformation("System templates seed completed. Seeded {Count} of {Total}.", seeded, templates.Count);
    }

    private static T LoadResource<T>(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(fileName));

        if (resourceName == null)
            return default;

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        var json = reader.ReadToEnd();

        return System.Text.Json.JsonSerializer.Deserialize<T>(json, new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });
    }
}

internal record SystemTemplateSeedData
{
    public string Name { get; init; } = null!;
    public int TypeTemplate { get; init; }
    public string Subject { get; init; } = null!;
    public string Body { get; init; } = null!;
    public List<string> Variables { get; init; } = [];
    public string From { get; init; } = null!;
    public string Alias { get; init; } = null!;
    public bool IsHtml { get; init; }
}
