using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDesignPlus.Net.Microservice.Emails.Default.Test.Validations;

/// <summary>
/// A class for validating startup services.
/// </summary>
public class StartupTest
{
    /// <summary>
    /// Validates that the startup services do not throw exceptions during initialization.
    /// </summary>
    [Theory]
    [Startup<Domain.Startup>]
    public void Sturtup_CheckNotThrowException_Domain(IStartup startup, Exception exception)
    {
        // Assert
        Assert.NotNull(startup);
        Assert.Null(exception);
    }

    /// <summary>
    /// Validates that the startup services do not throw exceptions during initialization.
    /// </summary>
    [Theory]
    [Startup<Application.Startup>]
    public void Sturtup_CheckNotThrowException_Application(IStartup startup, Exception exception)
    {
        // Assert
        Assert.NotNull(startup);
        Assert.Null(exception);
    }

    /// <summary>
    /// La capa Infrastructure arranca con las secciones que necesita: Email (el remitente de Graph) y FileStorage.
    /// </summary>
    /// <remarks>
    /// No usa [Startup&lt;&gt;]: ese atributo inicializa con una configuracion vacia, y este Startup exige, a proposito,
    /// las secciones Email y FileStorage. Sin ellas el micro no debe arrancar (ver la prueba siguiente).
    /// </remarks>
    [Fact]
    public void Startup_Infrastructure_InitializesWithItsRequiredSections()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Email:TenantId"] = "tenant",
                ["Email:ClientId"] = "client",
                ["Email:ClientSecret"] = "secret",
                ["FileStorage:Local:Enable"] = "true",
            })
            .Build();

        var exception = Record.Exception(() => new Infrastructure.Startup().Initialize(new ServiceCollection(), configuration));

        Assert.Null(exception);
    }

    /// <summary>
    /// Sin la seccion Email el micro no arranca: no hay remitente con el que enviar.
    /// </summary>
    [Fact]
    public void Startup_Infrastructure_WithoutEmailSection_Throws()
    {
        var configuration = new ConfigurationBuilder().Build();

        var exception = Record.Exception(() => new Infrastructure.Startup().Initialize(new ServiceCollection(), configuration));

        Assert.IsType<InvalidOperationException>(exception);
    }
}