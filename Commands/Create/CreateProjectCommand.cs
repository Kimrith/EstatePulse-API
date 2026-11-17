using EstatePulse.Data;
using EstatePulse.DTOs;
using EstatePulse.Models;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Commands.Create;

public class CreateProjectCommand : IRequest<ProjectDto>
{
    public CreateProjectDto Project { get; set; } = new();
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public CreateProjectCommandHandler(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async ValueTask<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Project;

        // 1. Check for duplicate slug
        var slugExists = await _context.Projects
            .AnyAsync(p => p.Slug.ToLower() == dto.Slug.ToLower(), cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException($"A project with slug '{dto.Slug}' already exists. Please provide a unique slug.");
        }

        string? savedImagePath = null;

        // 1. Handle IFormFile upload if provided
        if (dto.ThumbnailFile != null && dto.ThumbnailFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(dto.ThumbnailFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Invalid file type. Only JPG, PNG, and WEBP are allowed.");
            }

            // Fallback to "wwwroot" if WebRootPath is null (e.g., if wwwroot folder hasn't been created yet)
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", "projects");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ThumbnailFile.CopyToAsync(stream, cancellationToken);
            }

            savedImagePath = $"/uploads/projects/{uniqueFileName}";
        }

        // 2. Map DTO to Project entity
        var project = new Project
        {
            Name = dto.Name,
            Slug = dto.Slug,
            LocationAddress = dto.LocationAddress,
            City = dto.City,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            ThumbnailUrl = savedImagePath,
            Description = dto.Description,
            IsPublished = dto.IsPublished,
            CreatedAt = DateTime.UtcNow
        };

        // 3. Save to database
        _context.Projects.Add(project);
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