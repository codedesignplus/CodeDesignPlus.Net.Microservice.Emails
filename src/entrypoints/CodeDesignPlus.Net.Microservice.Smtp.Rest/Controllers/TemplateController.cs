using CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Commands.CreateTemplate;
using CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Commands.DeleteTemplate;
using CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Commands.UpdateTemplate;
using CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Queries.FindById;
using CodeDesignPlus.Net.Microservice.Smtp.Application.Template.Queries.GetAll;

namespace CodeDesignPlus.Net.Microservice.Smtp.Rest.Controllers;

/// <summary>
/// Controller for managing the Templates.
/// </summary>
/// <param name="mediator">Mediator instance for sending commands and queries.</param>
/// <param name="mapper">Mapper instance for mapping between DTOs and commands/queries.</param>
[Route("api/[controller]")]
[ApiController]
public class TemplateController(IMediator mediator, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all Templates.
    /// </summary>
    /// <param name="criteria">Criteria for filtering and sorting the Templates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of Templates.</returns>
    [HttpGet]
    public async Task<IActionResult> GetTemplates([FromQuery] C.Criteria criteria, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllQuery(criteria), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get a Template by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the Template.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The Template.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTemplateById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new FindByIdQuery(id), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Create a new Template.
    /// </summary>
    /// <param name="data">Data for creating the Template.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPost]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateDto data, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<CreateTemplateCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Update an existing Template.
    /// </summary>
    /// <param name="id">The unique identifier of the Template.</param>
    /// <param name="data">Data for updating the Template.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] UpdateTemplateDto data, CancellationToken cancellationToken)
    {
        data.Id = id;

        await mediator.Send(mapper.Map<UpdateTemplateCommand>(data), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete an existing Template.
    /// </summary>
    /// <param name="id">The unique identifier of the Template.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTemplateCommand(id), cancellationToken);

        return NoContent();
    }
}