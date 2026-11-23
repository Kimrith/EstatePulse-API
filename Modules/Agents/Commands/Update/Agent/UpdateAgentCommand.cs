using EstatePulse.Modules.Agents.DTOs;
using EstatePulse.Modules.Agents.DTOs.Agent;
using Mediator;

namespace EstatePulse.Modules.Agents.Commands.Update;

public sealed record UpdateAgentCommand(
    int Id,
    string? LicenseNumber = null,
    decimal? CommissionRate = null,
    string? AssignedBranch = null,
    int? TotalDealsClosed = null,
    decimal? Rating = null,
    string? Bio = null,
    bool? IsVerified = null
) : IRequest<AgentDto?>;
