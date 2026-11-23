using Mediator;

namespace EstatePulse.Modules.Units.Commands.Delete;

public sealed record DeleteUnitCommand(int Id) : IRequest<bool>;
