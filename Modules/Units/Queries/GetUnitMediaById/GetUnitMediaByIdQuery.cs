using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Queries;

public sealed record GetUnitMediaByIdQuery(int Id) : IRequest<UnitMediaDto?>;
