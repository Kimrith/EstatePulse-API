using EstatePulse.Modules.Users.DTOs;
using Mediator;

namespace EstatePulse.Modules.Users.Commands.Create;

public sealed record CreateUserFollowsCommand(
    int FollowingUserId,
    int FollowedUserId
) : IRequest<UserFollowsDto>;
