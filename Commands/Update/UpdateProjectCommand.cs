using EstatePulse.Data;
using EstatePulse.DTOs;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Commands.Update;

public class UpdateProjectCommand : IRequest<ProjectDto?>
{
    public int Id { get; set; }
    public UpdateProjectDto Project { get; set; } = new();
}

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto?>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public UpdateProjectCommandHandler(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async ValueTask<ProjectDto?> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project == null)
        {
            return null;
        }

        var dto = request.Project;

        // Check if new slug is already taken by another project
        var slugTaken = await _context.Projects
            .AnyAsync(p => p.Slug.ToLower() == dto.Slug.ToLower() && p.Id != request.Id, cancellationToken);

        if (slugTaken)
        {
            throw new InvalidOperationException($"A project with slug '{dto.Slug}' already exists. Please provide a unique slug.");
        }

        // 1. Replace image only if a new ThumbnailFile was uploaded
        if (dto.ThumbnailFile != null && dto.ThumbnailFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(dto.ThumbnailFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Invalid file type. Only JPG, PNG, and WEBP are allowed.");
            }

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", "projects");
            Directory.CreateDirectory(uploadsFolder);

            // Delete old image file if one exists
            if (!string.IsNullOrEmpty(project.ThumbnailUrl))
            {
                var relativeOldPath = project.ThumbnailUrl.TrimStart('/')
                    .Replace('/', Path.DirectorySeparatorChar);
                var oldFilePath = Path.Combine(webRoot, relativeOldPath);

                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            // Save new image file
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var newFilePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await dto.ThumbnailFile.CopyToAsync(stream, cancellationToken);
            }

            project.ThumbnailUrl = $"/uploads/projects/{uniqueFileName}";
        }

        // 2. Update remaining fields
        project.Name = dto.Name;
        project.Slug = dto.Slug;
        project.LocationAddress = dto.LocationAddress;
        project.City = dto.City;
        project.Latitude = dto.Latitude;
        project.Longitude = dto.Longitude;
        project.Description = dto.Description;
        project.IsPublished = dto.IsPublished;

        // 3. Persist changes
        await _context.SaveChangesAsync(cancellationToken);

        // 4. Return mapped ProjectDto
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Slug = project.Slug,
            LocationAddress = project.LocationAddress,
            City = project.City,
            Latitude = project.Latitude,
            Longitude = project.Longitude,
            ThumbnailUrl = project.ThumbnailUrl,
            Description = project.Description,
            IsPublished = project.IsPublished,
            CreatedAt = project.CreatedAt
        };
    }
}