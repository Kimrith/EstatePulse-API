using EstatePulse.Common.Results;
using EstatePulse.Modules.Projects.DTOs;
using Mediator;

namespace EstatePulse.Modules.Projects.Queries;

public sealed record GetProjectPhasesQuery(
    int? ProjectId = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<ProjectPhaseDto>>;
