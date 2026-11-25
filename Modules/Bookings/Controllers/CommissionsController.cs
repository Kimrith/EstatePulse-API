using EstatePulse.Common.Results;
using EstatePulse.Modules.Bookings.Commands.Create;
using EstatePulse.Modules.Bookings.Commands.Delete;
using EstatePulse.Modules.Bookings.Commands.Update;
using EstatePulse.Modules.Bookings.DTOs.Commissions;
using EstatePulse.Modules.Bookings.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Modules.Bookings.Controllers;

[Route("api/commissions")]
[ApiController]
public class CommissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/commissions?pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PaginatedList<CommissionDto>>> GetCommissions(
        [FromQuery] int? bookingId = null,
        [FromQuery] int? agentId = null,
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetPaginatedCommissionsQuery(
            bookingId,
            agentId,
            status,
            pageNumber,
            pageSize
        );

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // GET: api/commissions/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CommissionDto>> GetCommissionById(int id)
    {
        var result = await _mediator.Send(new GetCommissionByIdQuery(id));

        if (result == null)
        {
            return NotFound(new { message = $"Commission with ID {id} not found." });
        }

        return Ok(result);
    }

    // POST: api/commissions
    [HttpPost]
    public async Task<ActionResult<CommissionDto>> CreateCommission([FromBody] CommissionCreateDto dto)
    {
        try
        {
            var command = new CreateCommissionsCommand(
                dto.BookingId,
                dto.AgentId,
                dto.CommissionRate,
                dto.CommissionAmount,
                dto.Status
            );

            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetCommissionById), new { id = result.Id }, result);
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

    // PUT: api/commissions/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<CommissionDto>> UpdateCommission(int id, [FromBody] CommissionUpdateDto dto)
    {
        try
        {
            var command = new UpdateCommissionCommand(
                id,
                dto.CommissionRate,
                dto.CommissionAmount,
                dto.Status,
                dto.ApprovedByUserId,
                dto.ApprovedAt,
                dto.PaidAt
            );

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound(new { message = $"Commission with ID {id} not found." });
            }

            return Ok(result);
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

    // PATCH: api/commissions/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<CommissionDto>> UpdateCommissionStatus(int id, [FromBody] CommissionStatusUpdateDto dto)
    {
        try
        {
            var command = new UpdateCommissionStatusCommand(id, dto.Status, dto.ApprovedByUserId);
            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound(new { message = $"Commission with ID {id} not found." });
            }

            return Ok(result);
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

    // DELETE: api/commissions/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCommission(int id)
    {
        var success = await _mediator.Send(new DeleteCommissionsCommand(id));

        if (!success)
        {
            return NotFound(new { message = $"Commission with ID {id} not found." });
        }

        return NoContent();
    }
}
