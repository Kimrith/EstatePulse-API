using EstatePulse.Common.Results;
using EstatePulse.Modules.Agents.DTOs.Agent;
using Mediator;

namespace EstatePulse.Modules.Agents.Queries;

public sealed record GetPaginatedAgentsQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<AgentDto>>;
