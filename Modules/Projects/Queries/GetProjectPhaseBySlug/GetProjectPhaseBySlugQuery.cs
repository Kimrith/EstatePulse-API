using EstatePulse.Modules.Projects.DTOs;
using Mediator;

namespace EstatePulse.Modules.Projects.Queries;

public sealed record GetProjectPhaseBySlugQuery(int ProjectId, string Slug) : IRequest<ProjectPhaseDto?>;
