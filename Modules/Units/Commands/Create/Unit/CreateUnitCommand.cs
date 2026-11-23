using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Commands.Create;

public sealed record CreateUnitCommand(
    int PhaseId,
    int UnitTypeId,
    string UnitCode,
    string? StreetNumber,
    decimal LandAreaSqm,
    decimal BuiltAreaSqm,
    decimal BasePrice,
    string Status,
    string? SvgElementId
) : IRequest<UnitDto>;
