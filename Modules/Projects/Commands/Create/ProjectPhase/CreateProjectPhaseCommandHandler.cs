using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Projects.DTOs;
using EstatePulse.Modules.Projects.Models;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Projects.Commands.Create;

public sealed class CreateProjectPhaseCommandHandler : IRequestHandler<CreateProjectPhaseCommand, ProjectPhaseDto>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public CreateProjectPhaseCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<ProjectPhaseDto> Handle(CreateProjectPhaseCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify parent project exists
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {request.ProjectId} was not found.");
        }

        // 2. Check for duplicate slug within the same project
        var slugExists = await _context.ProjectPhases
            .AnyAsync(pp => pp.ProjectId == request.ProjectId && pp.Slug.ToLower() == request.Slug.ToLower(), cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException($"A phase with slug '{request.Slug}' already exists in project '{project.Name}'.");
        }

        string? savedSvgPath = null;

        // 3. Handle SVG file upload if provided
        if (request.MasterPlanSvgFile != null && request.MasterPlanSvgFile.Length > 0)
        {
            var allowedExtensions = new[] { ".svg", ".png", ".jpg", ".jpeg", ".webp" };
            var extension = Path.GetExtension(request.MasterPlanSvgFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Invalid file type. Only SVG, PNG, JPG, and WEBP are allowed for master plans.");
            }

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", "phases");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.MasterPlanSvgFile.CopyToAsync(stream, cancellationToken);
            }

            savedSvgPath = $"/uploads/phases/{uniqueFileName}";
        }

        // 4. Map to ProjectPhase entity
        var phase = new ProjectPhase
        {
            ProjectId = request.ProjectId,
            PhaseName = request.PhaseName,
            Slug = request.Slug,
            MasterPlanSvgUrl = savedSvgPath,
            LaunchDate = request.LaunchDate,
            ExpectedCompletionDate = request.ExpectedCompletionDate,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };

        _context.ProjectPhases.Add(phase);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("phases");

        return new ProjectPhaseDto
        {
            Id = phase.Id,
            ProjectId = phase.ProjectId,
            PhaseName = phase.PhaseName,
            Slug = phase.Slug,
            MasterPlanSvgUrl = phase.MasterPlanSvgUrl,
            LaunchDate = phase.LaunchDate,
            ExpectedCompletionDate = phase.ExpectedCompletionDate,
            Status = phase.Status,
            CreatedAt = phase.CreatedAt,
            ProjectName = project.Name
        };
    }
}
