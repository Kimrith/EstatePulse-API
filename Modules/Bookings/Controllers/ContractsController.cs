using EstatePulse.Common.Results;
using EstatePulse.Modules.Bookings.Commands.Create;
using EstatePulse.Modules.Bookings.Commands.Delete;
using EstatePulse.Modules.Bookings.Commands.Update;
using EstatePulse.Modules.Bookings.DTOs.Contract;
using EstatePulse.Modules.Bookings.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Modules.Bookings.Controllers;

[Route("api/contracts")]
[ApiController]
public class ContractsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContractsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/contracts?pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PaginatedList<ContractDto>>> GetContracts(
        [FromQuery] int? bookingId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetPaginatedContractsQuery(
            bookingId,
            status,
            searchTerm,
            pageNumber,
            pageSize
        );

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // GET: api/contracts/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ContractDto>> GetContractById(int id)
    {
        var result = await _mediator.Send(new GetContractByIdQuery(id));

        if (result == null)
        {
            return NotFound(new { message = $"Contract with ID {id} not found." });
        }

        return Ok(result);
    }

    // POST: api/contracts
    [HttpPost]
    public async Task<ActionResult<ContractDto>> CreateContract([FromBody] ContractCreateDto dto)
    {
        try
        {
            var command = new CreateContractCommand(
                dto.BookingId,
                dto.ContractNumber,
                dto.DocumentUrl,
                dto.Status
            );

            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetContractById), new { id = result.Id }, result);
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

    // PUT: api/contracts/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ContractDto>> UpdateContract(int id, [FromBody] ContractUpdateDto dto)
    {
        try
        {
            var command = new UpdateContractCommand(
                id,
                dto.ContractNumber,
                dto.DocumentUrl,
                dto.Status,
                dto.SignedAt
            );

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound(new { message = $"Contract with ID {id} not found." });
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PATCH: api/contracts/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ContractDto>> UpdateContractStatus(int id, [FromBody] ContractStatusUpdateDto dto)
    {
        var command = new UpdateContractStatusCommand(id, dto.Status);
        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound(new { message = $"Contract with ID {id} not found." });
        }

        return Ok(result);
    }

    // DELETE: api/contracts/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContract(int id)
    {
        var success = await _mediator.Send(new DeleteContractCommand(id));

        if (!success)
        {
            return NotFound(new { message = $"Contract with ID {id} not found." });
        }

        return NoContent();
    }
}
