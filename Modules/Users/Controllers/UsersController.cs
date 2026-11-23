using EstatePulse.Common.Results;
using EstatePulse.Modules.Users.Commands.Create;
using EstatePulse.Modules.Users.Commands.Delete;
using EstatePulse.Modules.Users.Commands.Update;
using EstatePulse.Modules.Users.DTOs;
using EstatePulse.Modules.Users.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Modules.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<UserDto>>> GetUsers(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUsersQuery(searchTerm, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        if (user == null)
        {
            return NotFound(new { message = $"User with ID {id} not found." });
        }

        return Ok(user);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UserDto>> CreateUser(
        [FromForm] UserCreateDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateUserCommand(
                dto.FullName,
                dto.Email,
                dto.PhoneNumber,
                dto.Password,
                dto.Role,
                dto.NationalIdCard,
                dto.Avatar
            );
            var createdUser = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
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

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UserDto>> UpdateUser(
        int id,
        [FromForm] UserUpdateDto userUpdateDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new UpdateUserCommand(
                id,
                userUpdateDto.FullName,
                userUpdateDto.Email,
                userUpdateDto.PhoneNumber,
                userUpdateDto.Password,
                userUpdateDto.Role,
                userUpdateDto.NationalIdCard,
                userUpdateDto.Avatar,
                userUpdateDto.IsActive
            );
            var updatedUser = await _mediator.Send(command, cancellationToken);
            if (updatedUser == null)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }

            return Ok(updatedUser);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new DeleteUserCommand(id), cancellationToken);
            if (!result)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
