using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Queries;

public sealed record GetUnitMediaByUnitTypeIdQuery(int UnitTypeId) : IRequest<List<UnitMediaDto>>;
