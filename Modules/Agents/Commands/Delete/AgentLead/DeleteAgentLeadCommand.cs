using Mediator;

namespace EstatePulse.Modules.Agents.Commands.Delete;

public sealed record DeleteAgentLeadCommand(int Id) : IRequest<bool>;
