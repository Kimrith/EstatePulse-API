using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Projects.DTOs;
using EstatePulse.Modules.Projects.Models;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Projects.Commands.Create;

public sealed class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public CreateProjectCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // 1. Check for duplicate slug
        var slugExists = await _context.Projects
            .AnyAsync(p => p.Slug.ToLower() == request.Slug.ToLower(), cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException($"A project with slug '{request.Slug}' already exists. Please provide a unique slug.");
        }

        string? savedImagePath = null;

        // 2. Handle IFormFile upload if provided
        if (request.ThumbnailFile != null && request.ThumbnailFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(request.ThumbnailFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Invalid file type. Only JPG, PNG, and WEBP are allowed.");
            }

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", "projects");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.ThumbnailFile.CopyToAsync(stream, cancellationToken);
            }

            savedImagePath = $"/uploads/projects/{uniqueFileName}";
        }

        // 3. Map to Project entity
        var project = new Project
        {
            Name = request.Name,
            Slug = request.Slug,
            LocationAddress = request.LocationAddress,
            City = request.City,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ThumbnailUrl = savedImagePath,
            Description = request.Description,
            IsPublished = request.IsPublished,
            CreatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("projects");

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
