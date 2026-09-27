using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Core.Abstractions;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Consumers;
using CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.DomainEvents;
using CodeDesignPlus.Net.Microservice.Emails.Domain;
using CodeDesignPlus.Net.Microservice.Emails.Domain.Repositories;
using CodeDesignPlus.Net.Mongo.Abstractions;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Emails.AsyncWorker.Test.Consumers;

/// <summary>
/// Al purgarse una copropiedad no puede quedar en este micro ningún documento suyo (regla 47).
/// </summary>
public class PurgeTenantDataHandlerTest
{
    /// <summary>
    /// Los tipos persistidos del micro que guardan la copropiedad en una propiedad <c>Tenant</c>.
    /// </summary>
    /// <remarks>
    /// Sale del dominio por reflexión y no de una lista escrita a mano: así un agregado nuevo entra solo en la
    /// prueba, y si nadie lo añade al consumidor, la prueba falla.
    /// </remarks>
    private static HashSet<Type> TypesWithTenant() => typeof(TemplateAggregate).Assembly.GetTypes()
        .Where(type => type.IsClass && !type.IsAbstract && typeof(IEntityBase).IsAssignableFrom(type))
        .Where(type => type.GetProperty("Tenant", BindingFlags.Public | BindingFlags.Instance)?.PropertyType is { } property && (property == typeof(Guid) || property == typeof(Guid?)))
        .ToHashSet();

    private static List<IInvocation> Purges(Mock<ITemplateRepository> repository) => repository.Invocations
        .Where(invocation => invocation.Method.Name == nameof(IRepositoryBase.DeleteByTenantAsync))
        .ToList();

    [Fact]
    public async Task HandleAsync_TenantPurged_DeletesEveryTypeWithTenant()
    {
        // Arrange
        var repository = new Mock<ITemplateRepository>();
        var tenant = Guid.NewGuid();
        var handler = new PurgeTenantDataHandler(repository.Object, Mock.Of<ILogger<PurgeTenantDataHandler>>());

        // Act
        await handler.HandleAsync(TenantPurgedDomainEvent.Create(tenant, "Malpelo XXI"), CancellationToken.None);

        // Assert
        var purged = Purges(repository)
            .Where(invocation => (Guid)invocation.Arguments[0] == tenant)
            .Select(invocation => invocation.Method.GetGenericArguments()[0])
            .ToHashSet();

        Assert.Empty(TypesWithTenant().Except(purged).Select(type => type.Name));
    }

    [Fact]
    public async Task HandleAsync_TenantPurged_NeverPurgesWithAnEmptyTenant()
    {
        // Arrange
        var repository = new Mock<ITemplateRepository>();
        var tenant = Guid.NewGuid();
        var handler = new PurgeTenantDataHandler(repository.Object, Mock.Of<ILogger<PurgeTenantDataHandler>>());

        // Act
        await handler.HandleAsync(TenantPurgedDomainEvent.Create(tenant, "Malpelo XXI"), CancellationToken.None);

        // Assert
        // Las plantillas de sistema tienen Tenant = null: solo se salvan si el borrado va siempre con el id de la copropiedad.
        Assert.NotEmpty(Purges(repository));
        Assert.All(Purges(repository), invocation => Assert.Equal(tenant, (Guid)invocation.Arguments[0]));
    }

    [Fact]
    public void TypesWithTenant_Domain_FindsTheAggregates()
    {
        // Act
        var types = TypesWithTenant();

        // Assert
        Assert.Contains(typeof(TemplateAggregate), types);
        Assert.Contains(typeof(EmailsAggregate), types);
    }
}
