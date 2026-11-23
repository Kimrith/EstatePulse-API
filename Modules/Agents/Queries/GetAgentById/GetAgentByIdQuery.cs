using EstatePulse.Modules.Agents.DTOs.Agent;
using Mediator;

namespace EstatePulse.Modules.Agents.Queries;

public sealed record GetAgentByIdQuery(int Id) : IRequest<AgentDto?>;
