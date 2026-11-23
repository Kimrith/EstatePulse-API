using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Projects.DTOs;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Projects.Commands.Update;

public sealed class UpdateProjectPhaseCommandHandler : IRequestHandler<UpdateProjectPhaseCommand, ProjectPhaseDto?>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public UpdateProjectPhaseCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<ProjectPhaseDto?> Handle(UpdateProjectPhaseCommand request, CancellationToken cancellationToken)
    {
        var phase = await _context.ProjectPhases
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (phase == null)
        {
            return null;
        }

        int targetProjectId = request.ProjectId ?? phase.ProjectId;

        if (request.ProjectId.HasValue && request.ProjectId.Value != phase.ProjectId)
        {
            var projectExists = await _context.Projects
                .AnyAsync(p => p.Id == request.ProjectId.Value, cancellationToken);
            if (!projectExists)
            {
                throw new KeyNotFoundException($"Project with ID {request.ProjectId.Value} was not found.");
            }
            phase.ProjectId = request.ProjectId.Value;
        }

        // Validate slug uniqueness if changed
        if (!string.IsNullOrWhiteSpace(request.Slug))
        {
            var slugTaken = await _context.ProjectPhases
                .AnyAsync(pp => pp.ProjectId == targetProjectId && pp.Slug.ToLower() == request.Slug.ToLower() && pp.Id != request.Id, cancellationToken);

            if (slugTaken)
            {
                throw new InvalidOperationException($"A phase with slug '{request.Slug}' already exists in this project.");
            }
            phase.Slug = request.Slug;
        }

        // Handle file upload if new master plan provided
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

            // Delete old file if present
            if (!string.IsNullOrEmpty(phase.MasterPlanSvgUrl))
            {
                var relativeOldPath = phase.MasterPlanSvgUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var oldFilePath = Path.Combine(webRoot, relativeOldPath);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var newFilePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await request.MasterPlanSvgFile.CopyToAsync(stream, cancellationToken);
            }

            phase.MasterPlanSvgUrl = $"/uploads/phases/{uniqueFileName}";
        }

        if (!string.IsNullOrWhiteSpace(request.PhaseName)) phase.PhaseName = request.PhaseName;
        if (request.LaunchDate.HasValue) phase.LaunchDate = request.LaunchDate.Value;
        if (request.ExpectedCompletionDate.HasValue) phase.ExpectedCompletionDate = request.ExpectedCompletionDate.Value;
        if (!string.IsNullOrWhiteSpace(request.Status)) phase.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("phases");

        var projectName = await _context.Projects
            .Where(p => p.Id == phase.ProjectId)
            .Select(p => p.Name)
            .FirstOrDefaultAsync(cancellationToken);

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
            ProjectName = projectName
        };
    }
}
