using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Queries;

public sealed record GetUnitTypeByCodeQuery(string Code) : IRequest<UnitTypeDto?>;
