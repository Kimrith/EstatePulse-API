using Mediator;

namespace EstatePulse.Modules.Agents.Commands.Delete;

public sealed record DeleteAgentCommand(int Id) : IRequest<bool>;
