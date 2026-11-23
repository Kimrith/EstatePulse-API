using EstatePulse.Common.Results;
using EstatePulse.Modules.Bookings.Commands.Create;
using EstatePulse.Modules.Bookings.Commands.Delete;
using EstatePulse.Modules.Bookings.Commands.Update;
using EstatePulse.Modules.Bookings.DTOs.Booking;
using EstatePulse.Modules.Bookings.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Modules.Bookings.Controllers;

[Route("api/bookings")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/bookings?pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PaginatedList<BookingDto>>> GetBookings(
        [FromQuery] int? unitId = null,
        [FromQuery] int? customerId = null,
        [FromQuery] int? agentId = null,
        [FromQuery] string? status = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetPaginatedBookingsQuery(
            unitId,
            customerId,
            agentId,
            status,
            pageNumber,
            pageSize
        );

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // GET: api/bookings/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetBookingById(int id)
    {
        var result = await _mediator.Send(new GetBookingByIdQuery(id));

        if (result == null)
        {
            return NotFound(new { message = $"Booking with ID {id} not found." });
        }

        return Ok(result);
    }

    // POST: api/bookings
    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking([FromBody] BookingCreateDto dto)
    {
        try
        {
            var command = new CreateBookingCommand(
                dto.UnitId,
                dto.CustomerId,
                dto.AgentId,
                dto.AgreedPrice,
                dto.DiscountAmount,
                dto.DepositRequired,
                dto.Status,
                dto.ReservationExpiresAt,
                dto.Notes
            );

            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetBookingById), new { id = result.Id }, result);
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

    // PUT: api/bookings/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<BookingDto>> UpdateBooking(int id, [FromBody] BookingUpdateDto dto)
    {
        try
        {
            var command = new UpdateBookingCommand(
                id,
                dto.AgentId,
                dto.AgreedPrice,
                dto.DiscountAmount,
                dto.DepositRequired,
                dto.Status,
                dto.ReservationExpiresAt,
                dto.Notes
            );

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound(new { message = $"Booking with ID {id} not found." });
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

    // PATCH: api/bookings/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<BookingDto>> UpdateBookingStatus(int id, [FromBody] BookingStatusUpdateDto dto)
    {
        var command = new UpdateBookingStatusCommand(id, dto.Status);
        var result = await _mediator.Send(command);

        if (result == null)
        {
            return NotFound(new { message = $"Booking with ID {id} not found." });
        }

        return Ok(result);
    }

    // DELETE: api/bookings/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        try
        {
            var success = await _mediator.Send(new DeleteBookingCommand(id));

            if (!success)
            {
                return NotFound(new { message = $"Booking with ID {id} not found." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
