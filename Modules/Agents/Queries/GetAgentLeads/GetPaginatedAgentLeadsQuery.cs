using EstatePulse.Common.Results;
using EstatePulse.Modules.Agents.DTOs.AgentLead;
using Mediator;

namespace EstatePulse.Modules.Agents.Queries;

public sealed record GetPaginatedAgentLeadsQuery(
    int? AgentId = null,
    string? Status = null,
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<AgentLeadDto>>;
