using EstatePulse.Modules.Projects.DTOs;
using Mediator;

namespace EstatePulse.Modules.Projects.Queries;

public sealed record GetProjectBySlugQuery(string Slug) : IRequest<ProjectDto?>;
