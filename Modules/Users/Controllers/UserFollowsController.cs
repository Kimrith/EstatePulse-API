using EstatePulse.Common.Results;
using EstatePulse.Modules.Users.Commands.Create;
using EstatePulse.Modules.Users.Commands.Delete;
using EstatePulse.Modules.Users.DTOs;
using EstatePulse.Modules.Users.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace EstatePulse.Modules.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserFollowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserFollowsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Follow a user
    /// </summary>
    [HttpPost("{followingUserId:int}")]
    public async Task<ActionResult<UserFollowsDto>> FollowUser(
        int followingUserId,
        [FromBody] FollowUserRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = new CreateUserFollowsCommand(followingUserId, request.FollowedUserId);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Unfollow by UserFollows relationship ID
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Unfollow(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteUserFollowsCommand(id), cancellationToken);
        if (!result)
        {
            return NotFound(new { message = "Follow relationship not found." });
        }

        return NoContent();
    }

    /// <summary>
    /// Get paginated list of followers for the specified user
    /// </summary>
    [HttpGet("followers/{userId:int}")]
    public async Task<ActionResult<PaginatedList<UserFollowsDto>>> GetFollowers(
        int userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetFollowersQuery(userId, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get paginated list of users that the specified user is following
    /// </summary>
    [HttpGet("following/{userId:int}")]
    public async Task<ActionResult<PaginatedList<UserFollowsDto>>> GetFollowing(
        int userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetFollowingQuery(userId, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Check if a user is following another user
    /// </summary>
    [HttpGet("is-following")]
    public async Task<ActionResult<object>> IsFollowing(
        [FromQuery] int followerId,
        [FromQuery] int followedId,
        CancellationToken cancellationToken = default)
    {
        var isFollowing = await _mediator.Send(new IsFollowingQuery(followerId, followedId), cancellationToken);
        return Ok(new { followerId, followedId, isFollowing });
    }

    /// <summary>
    /// Get follower and following counts for a user
    /// </summary>
    [HttpGet("counts/{userId:int}")]
    public async Task<ActionResult<UserFollowCountsDto>> GetCounts(int userId, CancellationToken cancellationToken = default)
    {
        var counts = await _mediator.Send(new GetUserFollowCountsQuery(userId), cancellationToken);
        return Ok(counts);
    }
}
