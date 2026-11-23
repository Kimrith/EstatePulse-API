using EstatePulse.Modules.Users.DTOs;
using Mediator;

namespace EstatePulse.Modules.Users.Queries;

public sealed record GetUserFollowCountsQuery(int UserId) : IRequest<UserFollowCountsDto>;
