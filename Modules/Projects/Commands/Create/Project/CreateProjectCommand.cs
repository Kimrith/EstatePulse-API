using EstatePulse.Modules.Projects.DTOs;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Projects.Commands.Create;

public sealed record CreateProjectCommand(
    string Name,
    string Slug,
    string? LocationAddress,
    string? City,
    decimal? Latitude,
    decimal? Longitude,
    IFormFile? ThumbnailFile,
    string? Description,
    bool IsPublished
) : IRequest<ProjectDto>;
