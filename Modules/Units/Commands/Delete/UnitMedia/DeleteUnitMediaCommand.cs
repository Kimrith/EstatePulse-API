using Mediator;

namespace EstatePulse.Modules.Units.Commands.Delete;

public sealed record DeleteUnitMediaCommand(int Id) : IRequest<bool>;
