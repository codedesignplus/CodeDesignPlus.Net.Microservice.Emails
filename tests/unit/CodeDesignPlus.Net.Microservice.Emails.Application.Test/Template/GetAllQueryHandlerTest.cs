using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.DataTransferObjects;
using CodeDesignPlus.Net.Microservice.Emails.Application.Template.Queries.GetAll;
using Xunit;

namespace CodeDesignPlus.Net.Microservice.Emails.Application.Test.Template;

public class GetAllQueryHandlerTest
{
    // El listado de plantillas devuelve todas de una vez: una sola pagina que empieza en 0 y trae tantas como hay.
    // Hasta el 2026-09-25 se construia con los argumentos cruzados y respondia limit 0 y skip = total (pendings/023).
    [Fact]
    public async Task Handle_ReturnsASinglePageStartingAtZeroWithEveryTemplate()
    {
        // Arrange
        var tenant = Guid.NewGuid();
        var repository = new Mock<ITemplateRepository>();
        var mapper = new Mock<IMapper>();
        var userContext = new Mock<IUserContext>();

        userContext.SetupGet(x => x.Tenant).Returns(tenant);
        repository.Setup(x => x.GetByTenantAsync(tenant, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        mapper.Setup(x => x.Map<List<TemplateDto>>(It.IsAny<object>())).Returns([new TemplateDto { Id = Guid.NewGuid() }, new TemplateDto { Id = Guid.NewGuid() }, new TemplateDto { Id = Guid.NewGuid() }]);

        var handler = new GetAllQueryHandler(repository.Object, mapper.Object, userContext.Object);

        // Act
        var result = await handler.Handle(new GetAllQuery(new C.Criteria()), CancellationToken.None);

        // Assert
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(0, result.Skip);
        Assert.Equal(3, result.Limit);
    }
}
