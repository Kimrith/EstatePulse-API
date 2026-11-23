using EstatePulse.Modules.Agents.DTOs.AgentLead;
using Mediator;

namespace EstatePulse.Modules.Agents.Queries;

public sealed record GetAgentLeadByIdQuery(int Id) : IRequest<AgentLeadDto?>;
