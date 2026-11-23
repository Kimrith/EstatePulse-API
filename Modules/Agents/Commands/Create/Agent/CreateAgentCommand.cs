using EstatePulse.Modules.Agents.DTOs;
using EstatePulse.Modules.Agents.DTOs.Agent;
using Mediator;

namespace EstatePulse.Modules.Agents.Commands.Create;

public sealed record CreateAgentCommand(
    int UserId,
    string AgentCode,
    string? LicenseNumber,
    decimal CommissionRate,
    string? AssignedBranch,
    int TotalDealsClosed,
    decimal? Rating,
    string? Bio,
    bool IsVerified
) : IRequest<AgentDto>;
