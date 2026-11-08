using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Microservice.Emails.Application.Setup;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.DeleteTemplate;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Commands.UpdateTemplate;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.FindById;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Test.Template;

/// <summary>
/// <c>GET Template/{id}</c> keeps the template in the cache for five minutes; editing or deleting it drops that copy
/// (pendings/674). Before, the Super Admin saved a change, read the old version back, and editing again overwrote the change.
/// </summary>
public class TemplateCacheTest
{
    private readonly Dictionary<string, object> cache = [];
    private readonly Mock<ICacheManager> cacheManager = new();
    private readonly Mock<ITemplateRepository> repository = new();
    private readonly TemplateAggregate template = TemplateAggregate.Create(Guid.NewGuid(), "PurchaseConfirmation", "Confirmación de Compra", "<p>Recibirás el comprobante en un correo separado.</p>", [], [], "gerencia@excellenceforge.com", "ExcellenceForge", true, null, Guid.NewGuid());

    public TemplateCacheTest()
    {
        MapsterConfigEmails.Configure();

        // A store with the behaviour of the cache: what is set is read back until it is removed.
        cacheManager.Setup(x => x.ExistsAsync(It.IsAny<string>())).ReturnsAsync((string key) => cache.ContainsKey(key));
        cacheManager.Setup(x => x.GetAsync<TemplateDto>(It.IsAny<string>())).ReturnsAsync((string key) => (TemplateDto)cache[key]);
        cacheManager.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<TemplateDto>(), It.IsAny<TimeSpan?>())).Callback((string key, TemplateDto value, TimeSpan? _) => cache[key] = value).Returns(Task.CompletedTask);
        cacheManager.Setup(x => x.RemoveAsync(It.IsAny<string>())).Callback((string key) => cache.Remove(key)).Returns(Task.CompletedTask);

        repository.Setup(x => x.FindAsync<TemplateAggregate>(template.Id, It.IsAny<CancellationToken>())).ReturnsAsync(template);
    }

    [Fact]
    public async Task FindById_AfterUpdate_ReturnsTheNewVersion()
    {
        // Arrange
        var before = await FindAsync();

        // Act
        await UpdateHandler().Handle(new UpdateTemplateCommand(template.Id, template.Name, template.Subject, "<p>El comprobante de pago va adjunto a este correo.</p>", [], [], template.From, template.Alias, true, null), CancellationToken.None);
        var read = await FindAsync();

        // Assert: the body as the template now stores it.
        Assert.Equal((template.Body, true), (read.Body, read.Body != before.Body));
    }

    [Fact]
    public async Task FindById_AfterDelete_DoesNotServeTheCachedCopy()
    {
        // Arrange
        await FindAsync();

        // Act
        await new DeleteTemplateCommandHandler(repository.Object, Mock.Of<IUserContext>(), Mock.Of<IPubSub>(), cacheManager.Object).Handle(new DeleteTemplateCommand(template.Id), CancellationToken.None);

        // Assert
        Assert.False(cache.ContainsKey(template.Id.ToString()));
    }

    private UpdateTemplateCommandHandler UpdateHandler() => new(repository.Object, Mock.Of<IUserContext>(), Mock.Of<IPubSub>(), cacheManager.Object);

    private Task<TemplateDto> FindAsync() => new FindByIdQueryHandler(repository.Object, new Mapper(TypeAdapterConfig.GlobalSettings), cacheManager.Object).Handle(new FindByIdQuery(template.Id), CancellationToken.None);
}
