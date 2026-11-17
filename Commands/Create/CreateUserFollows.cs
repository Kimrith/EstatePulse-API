using EstatePulse.Data;
using EstatePulse.DTOs;
using EstatePulse.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Commands.Create
{
    public class CreateUserFollowsCommand : IRequest<UserFollowsDto>
    {
        // Authenticated user performing the follow action
        public int FollowingUserId { get; set; }
        public FollowUserRequestDto UserFollow { get; set; } = new FollowUserRequestDto();

        public CreateUserFollowsCommand() { }

        public CreateUserFollowsCommand(int followingUserId, FollowUserRequestDto userFollow)
        {
            FollowingUserId = followingUserId;
            UserFollow = userFollow;
        }
    }

    public class CreateUserFollowsHandler : IRequestHandler<CreateUserFollowsCommand, UserFollowsDto>
    {
        private readonly AppDbContext _context;

        public CreateUserFollowsHandler(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<UserFollowsDto> Handle(CreateUserFollowsCommand request, CancellationToken cancellationToken)
        {
            int actorId = request.FollowingUserId;
            int targetId = request.UserFollow.FollowedUserId;

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
                .Include(uf => uf.FollowingUser)
                .Include(uf => uf.FollowedUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    uf => uf.FollowingUserId == actorId && uf.FollowedUserId == targetId,
                    cancellationToken
                );

            if (existing != null)
            {
                return MapToDto(existing);
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

            // 5. Load navigation properties for the DTO response
            await _context.Entry(newFollow)
                .Reference(f => f.FollowingUser)
                .LoadAsync(cancellationToken);

            await _context.Entry(newFollow)
                .Reference(f => f.FollowedUser)
                .LoadAsync(cancellationToken);

            return MapToDto(newFollow);
        }

        private static UserFollowsDto MapToDto(UserFollows entity)
        {
            return new UserFollowsDto
            {
                Id = entity.Id,
                FollowingUserId = entity.FollowingUserId,
                FollowedUserId = entity.FollowedUserId,
                CreatedAt = entity.CreatedAt,
                FollowingUser = entity.FollowingUser == null ? null : new UserSummaryDto
                {
                    Id = entity.FollowingUser.Id,
                    FullName = entity.FollowingUser.FullName,
                    Email = entity.FollowingUser.Email,
                    AvatarUrl = entity.FollowingUser.AvatarUrl
                },
                FollowedUser = entity.FollowedUser == null ? null : new UserSummaryDto
                {
                    Id = entity.FollowedUser.Id,
                    FullName = entity.FollowedUser.FullName,
                    Email = entity.FollowedUser.Email,
                    AvatarUrl = entity.FollowedUser.AvatarUrl
                }
            };
        }
    }
}