using EstatePulse.Commands.Create;
using EstatePulse.Commands.Delete;
using EstatePulse.Commands.Update;
using EstatePulse.Data;
using EstatePulse.DTOs;
using EstatePulse.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly AppDbContext _context;

    public UsersController(IMediator mediator, AppDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<UserDto>>> GetUsers(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var result = await UserQueries.GetUsersAsync(_context, searchTerm, pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id, CancellationToken cancellationToken)
    {
        var user = await UserQueries.GetUserByIdAsync(_context, id, cancellationToken);
        if (user == null)
        {
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        return Ok(user);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UserDto>> CreateUser(
        [FromForm] CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var createdUser = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UserDto>> UpdateUser(
        int id,
        [FromForm] UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        // Bind the route ID to the command to prevent mismatched form payloads
        command.Id = id;

        try
        {
            var updatedUser = await _mediator.Send(command, cancellationToken);
            if (updatedUser == null)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }

            return Ok(updatedUser);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteUserCommand(id), cancellationToken);
        if (!result)
        {
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        return NoContent();
    }
}