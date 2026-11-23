using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Queries;

public sealed record GetUnitMediaByUnitIdQuery(int UnitId) : IRequest<List<UnitMediaDto>>;
