using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.CreateConfigUserTemplate;
using CodeDesignPlus.Net.Microservice.Emails.Application.User.Commands.UpdateConfigUserTemplate;

namespace CodeDesignPlus.Net.Microservice.Emails.Rest.Controllers;

/// <summary>
/// Controller for managing the Users.
/// </summary>
/// <param name="mediator">Mediator instance for sending commands and queries.</param>
/// <param name="mapper">Mapper instance for mapping between DTOs and domain models.</param>
[Route("api/[controller]")]
[ApiController]
public class UserController(IMediator mediator, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Create a new User Template.
    /// </summary>
    /// <param name="data">Data for creating the User.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateConfigUserTemplateDto data, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<CreateConfigUserTemplateCommand>(data), cancellationToken);

        return NoContent();
    }
    
    /// <summary>
    /// Update an existing User Template.
    /// </summary>
    /// <param name="data">Data for updating the User Template.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateConfigUserTemplateDto data, CancellationToken cancellationToken)
    {
        await mediator.Send(mapper.Map<UpdateConfigUserTemplateCommand>(data), cancellationToken);

        return NoContent();
    }
}