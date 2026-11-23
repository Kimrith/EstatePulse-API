using EstatePulse.Modules.Units.Commands.Create;
using EstatePulse.Modules.Units.Commands.Delete;
using EstatePulse.Modules.Units.Commands.Update;
using EstatePulse.Modules.Units.DTOs;
using EstatePulse.Modules.Units.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Modules.Units.Controllers;

[ApiController]
[Route("api/unit-media")]
public class UnitMediaController : ControllerBase
{
    private readonly IMediator _mediator;

    public UnitMediaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/unit-media/unit/5
    [HttpGet("unit/{unitId:int}")]
    public async Task<ActionResult<List<UnitMediaDto>>> GetByUnitId(int unitId, CancellationToken cancellationToken = default)
    {
        var mediaList = await _mediator.Send(new GetUnitMediaByUnitIdQuery(unitId), cancellationToken);
        return Ok(mediaList);
    }

    // GET: api/unit-media/unit-type/2
    [HttpGet("unit-type/{unitTypeId:int}")]
    public async Task<ActionResult<List<UnitMediaDto>>> GetByUnitTypeId(int unitTypeId, CancellationToken cancellationToken = default)
    {
        var mediaList = await _mediator.Send(new GetUnitMediaByUnitTypeIdQuery(unitTypeId), cancellationToken);
        return Ok(mediaList);
    }

    // GET: api/unit-media/10
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UnitMediaDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var media = await _mediator.Send(new GetUnitMediaByIdQuery(id), cancellationToken);

        if (media == null)
        {
            return NotFound(new { message = $"Unit media with ID {id} was not found." });
        }

        return Ok(media);
    }

    // POST: api/unit-media (multipart/form-data)
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UnitMediaDto>> Create(
        [FromForm] CreateUnitMediaDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreateUnitMediaCommand(
                dto.UnitId,
                dto.UnitTypeId,
                dto.MediaType,
                dto.MediaFile,
                dto.MediaUrl,
                dto.SortOrder,
                dto.IsFeatured
            );
            var created = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // PUT: api/unit-media/10 (multipart/form-data)
    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UnitMediaDto>> Update(
        int id,
        [FromForm] UpdateUnitMediaDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new UpdateUnitMediaCommand(
                id,
                dto.UnitId,
                dto.UnitTypeId,
                dto.MediaType,
                dto.MediaFile,
                dto.MediaUrl,
                dto.SortOrder,
                dto.IsFeatured
            );

            var updated = await _mediator.Send(command, cancellationToken);

            if (updated == null)
            {
                return NotFound(new { message = $"Unit media with ID {id} was not found." });
            }

            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // DELETE: api/unit-media/10
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _mediator.Send(new DeleteUnitMediaCommand(id), cancellationToken);

        if (!deleted)
        {
            return NotFound(new { message = $"Unit media with ID {id} was not found." });
        }

        return NoContent();
    }
}
