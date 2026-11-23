using EstatePulse.Modules.Agents.DTOs;
using EstatePulse.Modules.Agents.DTOs.AgentLead;
using Mediator;

namespace EstatePulse.Modules.Agents.Commands.Create;

public sealed record CreateAgentLeadCommand(
    int AgentId,
    int? CustomerId,
    string LeadName,
    string LeadPhone,
    string? LeadEmail,
    decimal? BudgetMin,
    decimal? BudgetMax,
    string? Status,
    string? Notes
) : IRequest<AgentLeadDto>;
