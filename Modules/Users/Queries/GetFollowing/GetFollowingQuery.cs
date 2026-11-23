using EstatePulse.Common.Results;
using EstatePulse.Modules.Users.DTOs;
using Mediator;

namespace EstatePulse.Modules.Users.Queries;

public sealed record GetFollowingQuery(
    int UserId,
    int PageNumber = 1,
    int PageSize = 12
) : IRequest<PaginatedList<UserFollowsDto>>;
