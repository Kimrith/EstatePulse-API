using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Commands.Update;

public sealed record UpdateUnitCommand(
    int Id,
    int? PhaseId = null,
    int? UnitTypeId = null,
    string? UnitCode = null,
    string? StreetNumber = null,
    decimal? LandAreaSqm = null,
    decimal? BuiltAreaSqm = null,
    decimal? BasePrice = null,
    string? Status = null,
    string? SvgElementId = null
) : IRequest<UnitDto?>;
