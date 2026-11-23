using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Queries;

public sealed record GetUnitByCodeQuery(int PhaseId, string UnitCode) : IRequest<UnitDto?>;
