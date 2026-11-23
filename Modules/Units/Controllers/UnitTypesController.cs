using EstatePulse.Common.Results;
using EstatePulse.Modules.Units.Commands.Create;
using EstatePulse.Modules.Units.Commands.Delete;
using EstatePulse.Modules.Units.Commands.Update;
using EstatePulse.Modules.Units.DTOs;
using EstatePulse.Modules.Units.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Modules.Units.Controllers;

[ApiController]
[Route("api/unit-types")]
public class UnitTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public UnitTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/unit-types?searchTerm=villa&pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PaginatedList<UnitTypeDto>>> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUnitTypesQuery(searchTerm, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    // GET: api/unit-types/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UnitTypeDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var unitType = await _mediator.Send(new GetUnitTypeByIdQuery(id), cancellationToken);

        if (unitType == null)
        {
            return NotFound(new { message = $"Unit type with ID {id} was not found." });
        }

        return Ok(unitType);
    }

    // GET: api/unit-types/code/TYPE-A
    [HttpGet("code/{code}")]
    public async Task<ActionResult<UnitTypeDto>> GetByCode(string code, CancellationToken cancellationToken = default)
    {
        var unitType = await _mediator.Send(new GetUnitTypeByCodeQuery(code), cancellationToken);

        if (unitType == null)
        {
            return NotFound(new { message = $"Unit type with code '{code}' was not found." });
        }

        return Ok(unitType);
    }

    // POST: api/unit-types (multipart/form-data)
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UnitTypeDto>> Create(
        [FromForm] CreateUnitTypeDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreateUnitTypeCommand(
                dto.TypeName,
                dto.Code,
                dto.Bedrooms,
                dto.Bathrooms,
                dto.ParkingSlots,
                dto.Floors,
                dto.BuildingSize,
                dto.LandSizeRecommended,
                dto.FloorPlanImageFile,
                dto.SpecificationJson
            );
            var created = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT: api/unit-types/5 (multipart/form-data)
    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UnitTypeDto>> Update(
        int id,
        [FromForm] UpdateUnitTypeDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new UpdateUnitTypeCommand(
                id,
                dto.TypeName,
                dto.Code,
                dto.Bedrooms,
                dto.Bathrooms,
                dto.ParkingSlots,
                dto.Floors,
                dto.BuildingSize,
                dto.LandSizeRecommended,
                dto.FloorPlanImageFile,
                dto.SpecificationJson
            );

            var updated = await _mediator.Send(command, cancellationToken);

            if (updated == null)
            {
                return NotFound(new { message = $"Unit type with ID {id} was not found." });
            }

            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE: api/unit-types/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _mediator.Send(new DeleteUnitTypeCommand(id), cancellationToken);

            if (!deleted)
            {
                return NotFound(new { message = $"Unit type with ID {id} was not found." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
