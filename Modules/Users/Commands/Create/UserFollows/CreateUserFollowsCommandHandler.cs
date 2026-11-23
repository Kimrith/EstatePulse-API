using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Users.DTOs;
using EstatePulse.Modules.Users.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Users.Commands.Create;

public sealed class CreateUserFollowsCommandHandler : IRequestHandler<CreateUserFollowsCommand, UserFollowsDto>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;

    public CreateUserFollowsCommandHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async ValueTask<UserFollowsDto> Handle(CreateUserFollowsCommand request, CancellationToken cancellationToken)
    {
        int actorId = request.FollowingUserId;
        int targetId = request.FollowedUserId;

        // 1. Guard against self-following
        if (actorId == targetId)
        {
            throw new InvalidOperationException("Users cannot follow themselves.");
        }

        // 2. Validate that target user exists
        var targetUserExists = await _context.Users
            .AnyAsync(u => u.Id == targetId, cancellationToken);

        if (!targetUserExists)
        {
            throw new KeyNotFoundException($"User with ID {targetId} does not exist.");
        }

        // 3. Check for existing follow relationship
        var existing = await _context.UserFollows
            .AsNoTracking()
            .FirstOrDefaultAsync(
                uf => uf.FollowingUserId == actorId && uf.FollowedUserId == targetId,
                cancellationToken
            );

        if (existing != null)
        {
            return await BuildDtoAsync(existing, cancellationToken);
        }

        // 4. Create and persist record
        var newFollow = new UserFollows
        {
            FollowingUserId = actorId,
            FollowedUserId = targetId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.UserFollows.AddAsync(newFollow, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("user-followers");
        _cache.RemoveByPrefix("user-following");
        _cache.RemoveByPrefix("user-counts");

        return await BuildDtoAsync(newFollow, cancellationToken);
    }

    private async Task<UserFollowsDto> BuildDtoAsync(UserFollows entity, CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == entity.FollowingUserId || u.Id == entity.FollowedUserId)
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                AvatarUrl = u.AvatarUrl
            })
            .ToListAsync(cancellationToken);

        var followingSummary = users.FirstOrDefault(u => u.Id == entity.FollowingUserId);
        var followedSummary = users.FirstOrDefault(u => u.Id == entity.FollowedUserId);

        return new UserFollowsDto
        {
            Id = entity.Id,
            FollowingUserId = entity.FollowingUserId,
            FollowedUserId = entity.FollowedUserId,
            CreatedAt = entity.CreatedAt,
            FollowingUser = followingSummary,
            FollowedUser = followedSummary
        };
    }
}
