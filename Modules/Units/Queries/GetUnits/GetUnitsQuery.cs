using EstatePulse.Common.Results;
using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Queries;

public sealed record GetUnitsQuery(
    int? PhaseId = null,
    int? ProjectId = null,
    int? UnitTypeId = null,
    string? Status = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    int? MinBedrooms = null,
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<UnitDto>>;
