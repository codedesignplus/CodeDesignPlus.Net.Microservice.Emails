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
using AppErrors = CodeDesignPlus.Net.Microservice.Emails.Application.Errors;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Test.Template;

/// <summary>
/// <c>GET Template/{id}</c> keeps the template in the cache for five minutes; editing or deleting it drops that copy, and a
/// tenant only reads, edits or deletes its own copies and the global templates (pendings/674).
/// </summary>
/// <remarks>
/// Before, the Super Admin saved a change, read the old version back and, editing again, overwrote the change; and the key
/// was the bare id, so whoever knew the id of another tenant's copy read it.
/// </remarks>
public class TemplateCacheTest
{
    private static readonly Guid Tenant = Guid.Parse("e1000000-0000-4000-8000-000000000674");
    private static readonly Guid OtherTenant = Guid.Parse("e2000000-0000-4000-8000-000000000674");

    private readonly Dictionary<string, object> cache = [];
    private readonly Mock<ICacheManager> cacheManager = new();
    private readonly Mock<ITemplateRepository> repository = new();
    private readonly Mock<IUserContext> user = new();
    private readonly TemplateAggregate global = Build(null);
    private readonly TemplateAggregate copy = Build(Tenant);

    public TemplateCacheTest()
    {
        MapsterConfigEmails.Configure();

        // A store with the behaviour of the cache: what is set is read back until it is removed.
        cacheManager.Setup(x => x.ExistsAsync(It.IsAny<string>())).ReturnsAsync((string key) => cache.ContainsKey(key));
        cacheManager.Setup(x => x.GetAsync<TemplateDto>(It.IsAny<string>())).ReturnsAsync((string key) => (TemplateDto)cache[key]);
        cacheManager.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<TemplateDto>(), It.IsAny<TimeSpan?>())).Callback((string key, TemplateDto value, TimeSpan? _) => cache[key] = value).Returns(Task.CompletedTask);
        cacheManager.Setup(x => x.RemoveAsync(It.IsAny<string>())).Callback((string key) => cache.Remove(key)).Returns(Task.CompletedTask);

        repository.Setup(x => x.FindAsync<TemplateAggregate>(global.Id, It.IsAny<CancellationToken>())).ReturnsAsync(global);
        repository.Setup(x => x.FindAsync<TemplateAggregate>(copy.Id, It.IsAny<CancellationToken>())).ReturnsAsync(copy);

        SessionOf(Tenant);
    }

    [Fact]
    public async Task FindById_AfterUpdate_ReturnsTheNewVersion()
    {
        // Arrange
        var before = await FindAsync(copy.Id);

        // Act
        await UpdateAsync(copy);
        var read = await FindAsync(copy.Id);

        // Assert: the body as the template now stores it.
        Assert.Equal((copy.Body, true), (read.Body, read.Body != before.Body));
    }

    [Fact]
    public async Task FindById_GlobalAfterUpdate_ReturnsTheNewVersion()
    {
        // Arrange
        var before = await FindAsync(global.Id);

        // Act
        await UpdateAsync(global);
        var read = await FindAsync(global.Id);

        // Assert
        Assert.Equal((global.Body, true), (read.Body, read.Body != before.Body));
    }

    [Fact]
    public async Task FindById_AfterDelete_DoesNotServeTheCachedCopy()
    {
        // Arrange
        await FindAsync(copy.Id);

        // Act
        await new DeleteTemplateCommandHandler(repository.Object, user.Object, Mock.Of<IPubSub>(), cacheManager.Object).Handle(new DeleteTemplateCommand(copy.Id), CancellationToken.None);

        // Assert
        Assert.Empty(cache);
    }

    [Fact]
    public async Task FindById_CopyOfAnotherTenant_AnswersTemplateNotFound()
    {
        // Arrange
        SessionOf(OtherTenant);

        // Act
        var error = await Assert.ThrowsAsync<CodeDesignPlusException>(() => FindAsync(copy.Id));

        // Assert
        Assert.Equal(AppErrors.TemplateNotFound.GetCode(), error.Code);
    }

    [Fact]
    public async Task FindById_CopyCachedForItsTenant_IsNotServedToAnother()
    {
        // Arrange: its own tenant read it, so it is in the cache.
        await FindAsync(copy.Id);
        SessionOf(OtherTenant);

        // Act
        var error = await Assert.ThrowsAsync<CodeDesignPlusException>(() => FindAsync(copy.Id));

        // Assert
        Assert.Equal(AppErrors.TemplateNotFound.GetCode(), error.Code);
    }

    [Fact]
    public async Task FindById_GlobalTemplate_IsReadFromAnyTenant()
    {
        // Arrange
        SessionOf(OtherTenant);

        // Act
        var dto = await FindAsync(global.Id);

        // Assert
        Assert.Equal(global.Id, dto.Id);
    }

    [Fact]
    public async Task Update_CopyOfAnotherTenant_AnswersTemplateNotFoundAndChangesNothing()
    {
        // Arrange
        SessionOf(OtherTenant);
        var body = copy.Body;

        // Act
        var error = await Assert.ThrowsAsync<CodeDesignPlusException>(() => UpdateAsync(copy));

        // Assert
        Assert.Equal((AppErrors.TemplateNotFound.GetCode(), body), (error.Code, copy.Body));
    }

    [Fact]
    public async Task Delete_CopyOfAnotherTenant_AnswersTemplateNotFound()
    {
        // Arrange
        SessionOf(OtherTenant);

        // Act
        var error = await Assert.ThrowsAsync<CodeDesignPlusException>(() => new DeleteTemplateCommandHandler(repository.Object, user.Object, Mock.Of<IPubSub>(), cacheManager.Object).Handle(new DeleteTemplateCommand(copy.Id), CancellationToken.None));

        // Assert
        Assert.Equal(AppErrors.TemplateNotFound.GetCode(), error.Code);
        repository.Verify(x => x.DeleteAsync<TemplateAggregate>(copy.Id, It.IsAny<CancellationToken>()), Times.Never);
    }

    private static TemplateAggregate Build(Guid? tenant) => TemplateAggregate.Create(Guid.NewGuid(), "PurchaseConfirmation", "Confirmación de Compra", "<p>Recibirás el comprobante en un correo separado.</p>", [], [], "gerencia@excellenceforge.com", "ExcellenceForge", true, tenant, Guid.NewGuid());

    private void SessionOf(Guid tenant) => user.SetupGet(x => x.Tenant).Returns(tenant);

    private Task UpdateAsync(TemplateAggregate template) => new UpdateTemplateCommandHandler(repository.Object, user.Object, Mock.Of<IPubSub>(), cacheManager.Object).Handle(new UpdateTemplateCommand(template.Id, template.Name, template.Subject, "<p>El comprobante de pago va adjunto a este correo.</p>", [], [], template.From, template.Alias, true, null), CancellationToken.None);

    private Task<TemplateDto> FindAsync(Guid id) => new FindByIdQueryHandler(repository.Object, new Mapper(TypeAdapterConfig.GlobalSettings), cacheManager.Object, user.Object).Handle(new FindByIdQuery(id), CancellationToken.None);
}
