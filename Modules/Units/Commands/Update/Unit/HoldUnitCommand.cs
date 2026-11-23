using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Commands.Update;

public sealed record HoldUnitCommand(
    int Id,
    int UserId,
    int HoldMinutes = 15,
    string? RowVersion = null
) : IRequest<UnitDto>;
