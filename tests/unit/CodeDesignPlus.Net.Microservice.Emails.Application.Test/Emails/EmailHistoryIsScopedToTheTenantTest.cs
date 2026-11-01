using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Cache.Abstractions;
using CodeDesignPlus.Net.Core.Abstractions.Models.Pager;
using CodeDesignPlus.Net.Microservice.Emails.Application.Emails;
using CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Queries.GetAllEmails;
using CodeDesignPlus.Net.Microservice.Emails.Application.Emails.Queries.GetEmailsById;
using CodeDesignPlus.Net.Microservice.Emails.Application.Setup;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Test.Emails;

/// <summary>
/// El historial de correos solo lee los de la copropiedad que lo pide (pendings/680).
/// </summary>
/// <remarks>
/// <see cref="EmailsAggregate"/> no hereda de <c>AggregateRoot</c>, así que el repositorio no filtra solo por
/// copropiedad: el listado y el detalle le mostraban a cualquier administrador los correos de toda la plataforma, y el
/// detalle cacheado se servía por su id a cualquiera.
/// </remarks>
public class EmailHistoryIsScopedToTheTenantTest
{
    private static readonly Guid Acacias = Guid.Parse("274bb0a1-0000-0000-0000-000000000001");
    private static readonly Guid Almendros = Guid.Parse("17daab5e-b090-48bd-969a-a7618eb2a726");

    private readonly Mock<IEmailsRepository> repository = new();
    private readonly Mock<ICacheManager> cache = new();

    [Theory]
    [InlineData(null, "tenant=274bb0a1-0000-0000-0000-000000000001")]
    [InlineData("subject~=Cuenta", "subject~=Cuenta|and|tenant=274bb0a1-0000-0000-0000-000000000001")]
    [InlineData("subject~=a|or|subject~=b", "subject~=a|or|subject~=b|and|tenant=274bb0a1-0000-0000-0000-000000000001")]
    public void Scope_ClientFilter_EndsWithTheTenant(string? filters, string expected)
    {
        // Act
        var scoped = TenantScopedCriteria.Scope(Acacias, new C.Criteria { Filters = filters, OrderBy = "createdAt", Skip = 10, Limit = 10 });

        // Assert
        Assert.Equal(expected, scoped.Filters);
        Assert.Equal("createdAt", scoped.OrderBy);
        Assert.Equal(10, scoped.Skip);
        Assert.Equal(10, scoped.Limit);
    }

    [Fact]
    public async Task GetAll_Tenant_QueriesOnlyItsEmails()
    {
        // Arrange
        C.Criteria? queried = null;
        repository.Setup(x => x.MatchingAsync<EmailsAggregate>(It.IsAny<C.Criteria>(), It.IsAny<CancellationToken>()))
            .Callback((C.Criteria c, CancellationToken _) => queried = c)
            .ReturnsAsync(Pagination<EmailsAggregate>.Create([], 0, 10, 0));

        // Act
        await new GetAllEmailsQueryHandler(repository.Object, Mapper(), User(Acacias))
            .Handle(new GetAllEmailsQuery(new C.Criteria { Limit = 10 }), CancellationToken.None);

        // Assert
        Assert.Equal($"tenant={Acacias}", queried!.Filters);
    }

    [Fact]
    public async Task GetAll_WithoutTenant_ReturnsNothingAndDoesNotQuery()
    {
        // Act
        var page = await new GetAllEmailsQueryHandler(repository.Object, Mapper(), User(Guid.Empty))
            .Handle(new GetAllEmailsQuery(new C.Criteria { Limit = 10 }), CancellationToken.None);

        // Assert
        Assert.Empty(page.Data);
        repository.Verify(x => x.MatchingAsync<EmailsAggregate>(It.IsAny<C.Criteria>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetById_EmailOfAnotherTenant_IsNotFound()
    {
        // Arrange
        var email = Email(Almendros);
        repository.Setup(x => x.FindAsync<EmailsAggregate>(email.Id, It.IsAny<CancellationToken>())).ReturnsAsync(email);

        // Act
        var exception = await Assert.ThrowsAsync<CodeDesignPlusException>(() =>
            new GetEmailsByIdQueryHandler(repository.Object, Mapper(), cache.Object, User(Acacias))
                .Handle(new GetEmailsByIdQuery(email.Id), CancellationToken.None));

        // Assert
        Assert.Equal(Errors.EmailNotFound.Code, exception.Code);
        cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<EmailsDto>(), It.IsAny<TimeSpan?>()), Times.Never);
    }

    [Fact]
    public async Task GetById_PlatformEmail_IsNotFound()
    {
        // Arrange
        var email = Email(Guid.Empty);
        repository.Setup(x => x.FindAsync<EmailsAggregate>(email.Id, It.IsAny<CancellationToken>())).ReturnsAsync(email);

        // Act & Assert
        await Assert.ThrowsAsync<CodeDesignPlusException>(() =>
            new GetEmailsByIdQueryHandler(repository.Object, Mapper(), cache.Object, User(Acacias))
                .Handle(new GetEmailsByIdQuery(email.Id), CancellationToken.None));
    }

    [Fact]
    public async Task GetById_OwnEmail_IsCachedUnderTheTenant()
    {
        // Arrange
        var email = Email(Acacias);
        repository.Setup(x => x.FindAsync<EmailsAggregate>(email.Id, It.IsAny<CancellationToken>())).ReturnsAsync(email);

        // Act
        var dto = await new GetEmailsByIdQueryHandler(repository.Object, Mapper(), cache.Object, User(Acacias))
            .Handle(new GetEmailsByIdQuery(email.Id), CancellationToken.None);

        // Assert
        Assert.Equal(email.Id, dto.Id);
        cache.Verify(x => x.SetAsync($"{Acacias}:{email.Id}", It.IsAny<EmailsDto>(), It.IsAny<TimeSpan?>()), Times.Once);
    }

    private static IUserContext User(Guid tenant)
    {
        var user = new Mock<IUserContext>();
        user.SetupGet(x => x.Tenant).Returns(tenant);

        return user.Object;
    }

    private static EmailsAggregate Email(Guid tenant) =>
        EmailsAggregate.Create(Guid.NewGuid(), ["ana@kappali.com"], [], [], "Aviso", "PGgxPkhvbGE8L2gxPg==", "no-reply@kappali.com", [], true,
            new Dictionary<string, string> { { "title", "Aviso" } }, "202", null, tenant, true);

    private static Mapper Mapper()
    {
        MapsterConfigEmails.Configure();

        return new Mapper(TypeAdapterConfig.GlobalSettings);
    }
}
