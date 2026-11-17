using EstatePulse.Commands.Create;
using EstatePulse.Commands.Delete;
using EstatePulse.Data;
using EstatePulse.DTOs;
using EstatePulse.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace EstatePulse.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserFollowsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    public UserFollowsController(IMediator mediator, AppDbContext context, IMemoryCache cache)
    {
        _mediator = mediator;
        _context = context;
        _cache = cache;
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
            var command = new CreateUserFollowsCommand(followingUserId, request);
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
            return NotFound(new { message = $"Follow relationship with ID {id} not found." });
        }

        return NoContent();
    }

    /// <summary>
    /// Get paginated list of users following the specified user
    /// </summary>
    [HttpGet("followers/{userId:int}")]
    public async Task<ActionResult<PaginatedList<UserFollowsDto>>> GetFollowers(
        int userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken cancellationToken = default)
    {
        var result = await UserFollowsQueries.GetFollowersAsync(_context, userId, pageNumber, pageSize, _cache, cancellationToken);
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
        var result = await UserFollowsQueries.GetFollowingAsync(_context, userId, pageNumber, pageSize, _cache, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Check if a user is following another user
    /// </summary>
    [HttpGet("is-following")]
    public async Task<ActionResult<bool>> IsFollowing(
        [FromQuery] int followerId,
        [FromQuery] int followedId,
        CancellationToken cancellationToken = default)
    {
        var isFollowing = await UserFollowsQueries.IsFollowingAsync(_context, followerId, followedId, _cache, cancellationToken);
        return Ok(new { followerId, followedId, isFollowing });
    }

    /// <summary>
    /// Get follower and following counts for a user
    /// </summary>
    [HttpGet("counts/{userId:int}")]
    public async Task<ActionResult> GetCounts(int userId, CancellationToken cancellationToken = default)
    {
        var followersCount = await UserFollowsQueries.GetFollowersCountAsync(_context, userId, _cache, cancellationToken);
        var followingCount = await UserFollowsQueries.GetFollowingCountAsync(_context, userId, _cache, cancellationToken);

        return Ok(new
        {
            userId,
            followersCount,
            followingCount
        });
    }
}

