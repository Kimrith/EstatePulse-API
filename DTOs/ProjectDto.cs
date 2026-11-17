using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.DTOs;

// 1. Response DTO (Read): Returns the saved image URL path to the client
public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LocationAddress { get; set; }
    public string? City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Description { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}

// 2. Create DTO (Write): Accepts IFormFile only, no ThumbnailUrl allowed
public class CreateProjectDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LocationAddress { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public IFormFile? ThumbnailFile { get; set; }

    public string? Description { get; set; }

    public bool IsPublished { get; set; } = false;
}

// 3. Update DTO (Write): Accepts IFormFile only, no ThumbnailUrl allowed
public class UpdateProjectDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LocationAddress { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    // If provided, replaces the existing image; if null, keeps the current image
    public IFormFile? ThumbnailFile { get; set; }

    public string? Description { get; set; }

    public bool IsPublished { get; set; }
}