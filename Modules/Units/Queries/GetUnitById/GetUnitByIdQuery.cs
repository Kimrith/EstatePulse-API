using EstatePulse.Modules.Units.DTOs;
using Mediator;

namespace EstatePulse.Modules.Units.Queries;

public sealed record GetUnitByIdQuery(int Id) : IRequest<UnitDto?>;
