using EstatePulse.Common.Results;
using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Queries;

public sealed record GetUnitTypesQuery(
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<UnitTypeDto>>;
