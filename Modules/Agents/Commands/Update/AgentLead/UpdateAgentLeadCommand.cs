using EstatePulse.Modules.Agents.DTOs.AgentLead;
using Mediator;

namespace EstatePulse.Modules.Agents.Commands.Update;

public sealed record UpdateAgentLeadCommand(
    int Id,
    int? CustomerId = null,
    string? LeadName = null,
    string? LeadPhone = null,
    string? LeadEmail = null,
    decimal? BudgetMin = null,
    decimal? BudgetMax = null,
    string? Status = null,
    string? Notes = null
) : IRequest<AgentLeadDto?>;
