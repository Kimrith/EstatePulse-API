using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Commands.Update;

public sealed record ReleaseUnitHoldCommand(
    int Id,
    int? UserId = null
) : IRequest<UnitDto>;
