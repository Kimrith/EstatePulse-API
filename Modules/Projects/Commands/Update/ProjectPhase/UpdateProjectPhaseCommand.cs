using EstatePulse.Modules.Projects.DTOs;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Projects.Commands.Update;

public sealed record UpdateProjectPhaseCommand(
    int Id,
    int? ProjectId,
    string? PhaseName,
    string? Slug,
    IFormFile? MasterPlanSvgFile,
    DateTime? LaunchDate,
    DateTime? ExpectedCompletionDate,
    string? Status
) : IRequest<ProjectPhaseDto?>;
