using Mediator;

namespace EstatePulse.Modules.Units.Commands.Delete;

public sealed record DeleteUnitTypeCommand(int Id) : IRequest<bool>;
