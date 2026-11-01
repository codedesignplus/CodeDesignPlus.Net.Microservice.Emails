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
                {
                    await DeclareSensitiveVariablesAsync(templateRepository, data, stoppingToken);
                    continue;
                }

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
                    SystemUserId,
                    data.SensitiveVariables
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

    /// <summary>
    /// Las plantillas sembradas antes del 680 no tenían variables sensibles, ni sus copias por copropiedad. Se declaran
    /// las de la siembra en las que aún no tienen ninguna; las que una copropiedad ya marcó se respetan.
    /// </summary>
    private async Task DeclareSensitiveVariablesAsync(ITemplateRepository templateRepository, SystemTemplateSeedData data, CancellationToken stoppingToken)
    {
        if (data.SensitiveVariables.Count == 0)
            return;

        var templates = await templateRepository.GetByNameAsync(data.Name, stoppingToken);

        foreach (var template in templates.Where(template => template.SensitiveVariables.Count == 0))
        {
            var declared = data.SensitiveVariables.Where(template.Variables.Contains).ToList();

            if (declared.Count == 0)
                continue;

            template.DeclareSensitiveVariables(declared, SystemUserId);

            await templateRepository.UpdateAsync(template, stoppingToken);

            logger.LogInformation("Declared sensitive variables of template {Name} for tenant {Tenant}.", template.Name, template.Tenant);
        }
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
    public List<string> SensitiveVariables { get; init; } = [];
    public string From { get; init; } = null!;
    public string Alias { get; init; } = null!;
    public bool IsHtml { get; init; }
}
