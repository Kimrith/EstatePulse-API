using EstatePulse.Modules.Projects.DTOs;
using Mediator;

namespace EstatePulse.Modules.Projects.Queries;

public sealed record GetProjectPhaseByIdQuery(int Id) : IRequest<ProjectPhaseDto?>;
