using EstatePulse.Common.Results;
using EstatePulse.Modules.Users.DTOs;
using Mediator;

namespace EstatePulse.Modules.Users.Queries;

public sealed record GetUsersQuery(
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 12
) : IRequest<PaginatedList<UserDto>>;
