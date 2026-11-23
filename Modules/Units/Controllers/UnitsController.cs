using EstatePulse.Common.Results;
using EstatePulse.Modules.Units.Commands.Create;
using EstatePulse.Modules.Units.Commands.Delete;
using EstatePulse.Modules.Units.Commands.Update;
using EstatePulse.Modules.Units.DTOs;
using EstatePulse.Modules.Units.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Controllers;

[ApiController]
[Route("api/units")]
public class UnitsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UnitsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/units?phaseId=1&status=Available&minPrice=50000&maxPrice=200000&pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PaginatedList<UnitDto>>> GetAll(
        [FromQuery] int? phaseId = null,
        [FromQuery] int? projectId = null,
        [FromQuery] int? unitTypeId = null,
        [FromQuery] string? status = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int? minBedrooms = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUnitsQuery(
            phaseId,
            projectId,
            unitTypeId,
            status,
            minPrice,
            maxPrice,
            minBedrooms,
            searchTerm,
            pageNumber,
            pageSize
        );

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    // GET: api/units/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UnitDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var unit = await _mediator.Send(new GetUnitByIdQuery(id), cancellationToken);

        if (unit == null)
        {
            return NotFound(new { message = $"Unit with ID {id} was not found." });
        }

        return Ok(unit);
    }

    // GET: api/units/phase/1/code/PH1-A-01
    [HttpGet("phase/{phaseId:int}/code/{unitCode}")]
    public async Task<ActionResult<UnitDto>> GetByCode(int phaseId, string unitCode, CancellationToken cancellationToken = default)
    {
        var unit = await _mediator.Send(new GetUnitByCodeQuery(phaseId, unitCode), cancellationToken);

        if (unit == null)
        {
            return NotFound(new { message = $"Unit '{unitCode}' in phase {phaseId} was not found." });
        }

        return Ok(unit);
    }

    // POST: api/units
    [HttpPost]
    public async Task<ActionResult<UnitDto>> Create(
        [FromBody] CreateUnitDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreateUnitCommand(
                dto.PhaseId,
                dto.UnitTypeId,
                dto.UnitCode,
                dto.StreetNumber,
                dto.LandAreaSqm,
                dto.BuiltAreaSqm,
                dto.BasePrice,
                dto.Status,
                dto.SvgElementId
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
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT: api/units/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UnitDto>> Update(
        int id,
        [FromBody] UpdateUnitDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new UpdateUnitCommand(
                id,
                dto.PhaseId,
                dto.UnitTypeId,
                dto.UnitCode,
                dto.StreetNumber,
                dto.LandAreaSqm,
                dto.BuiltAreaSqm,
                dto.BasePrice,
                dto.Status,
                dto.SvgElementId
            );

            var updated = await _mediator.Send(command, cancellationToken);

            if (updated == null)
            {
                return NotFound(new { message = $"Unit with ID {id} was not found." });
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
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // POST: api/units/5/hold
    [HttpPost("{id:int}/hold")]
    public async Task<ActionResult<UnitDto>> Hold(
        int id,
        [FromBody] HoldUnitDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new HoldUnitCommand(
                id,
                dto.UserId,
                dto.HoldMinutes,
                dto.RowVersion
            );

            var held = await _mediator.Send(command, cancellationToken);
            return Ok(held);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Conflict(new
            {
                message = "Concurrency conflict: unit status or rowversion has changed since it was retrieved. Please refresh and try again.",
                details = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST: api/units/5/release-hold
    [HttpPost("{id:int}/release-hold")]
    public async Task<ActionResult<UnitDto>> ReleaseHold(
        int id,
        [FromBody] ReleaseUnitHoldDto? dto = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new ReleaseUnitHoldCommand(
                id,
                dto?.UserId
            );

            var released = await _mediator.Send(command, cancellationToken);
            return Ok(released);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/units/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _mediator.Send(new DeleteUnitCommand(id), cancellationToken);

            if (!deleted)
            {
                return NotFound(new { message = $"Unit with ID {id} was not found." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
