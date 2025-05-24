using CodeDesignPlus.Net.Microservice.Smtp.Application.Emails.Commands.SendEmail;
using CodeDesignPlus.Net.Microservice.Smtp.Domain.Models;
using CodeDesignPlus.Net.Microservice.Smtp.Rest.DataTransferObjects;

namespace CodeDesignPlus.Net.Microservice.Smtp.Rest.Controllers;

/// <summary>
/// Controller for managing the Emails.
/// </summary>
/// <param name="mediator">Mediator instance for sending commands and queries.</param>
/// <param name="mapper">Mapper instance for mapping between DTOs and commands/queries.</param>
[Route("api/[controller]")]
[ApiController]
public class EmailController(IMediator mediator, IMapper mapper) : ControllerBase
{
    /// <summary>
    /// Get all Emails.
    /// </summary>
    /// <param name="criteria">Criteria for filtering and sorting the Emails.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of Emails.</returns>
    // [HttpGet]
    // public async Task<IActionResult> GetEmails([FromQuery] C.Criteria criteria, CancellationToken cancellationToken)
    // {
    //     var result = await mediator.Send(new GetAllQuery(criteria), cancellationToken);

    //     return Ok(result);
    // }

    // /// <summary>
    // /// Get a Email by its ID.
    // /// </summary>
    // /// <param name="id">The unique identifier of the Email.</param>
    // /// <param name="cancellationToken">Cancellation token.</param>
    // /// <returns>The Email.</returns>
    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetEmailById(Guid id, CancellationToken cancellationToken)
    // {
    //     var result = await mediator.Send(new FindByIdQuery(id), cancellationToken);

    //     return Ok(result);
    // }

    /// <summary>
    /// Create a new Email.
    /// </summary>
    /// <param name="data">Data for creating the Email.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP status code 204 (No Content).</returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> SendEmail([FromForm] SendEmailDto data, CancellationToken cancellationToken)
    {
        var command = new SendEmailCommand(
            data.Id,
            data.IdTemplate,
            data.To,
            data.Cc,
            data.Bcc,
            data.Subject,
            ConvertToAttachments(data.Attachments),
            data.Values
        );

        await mediator.Send(command, cancellationToken);

        return NoContent();
    }
    

    private static List<Attachment> ConvertToAttachments(List<IFormFile> attachments)
    {
        var result = new List<Attachment>();

        foreach (var attachment in attachments)
        {
            using var stream = new MemoryStream();
            attachment.CopyTo(stream);
            var fileBytes = stream.ToArray();

            result.Add(Attachment.Create(attachment.FileName, attachment.ContentType, fileBytes));
        }

        return result;
    }
}