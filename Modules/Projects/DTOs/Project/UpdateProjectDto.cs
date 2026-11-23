using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Projects.DTOs;

public sealed class UpdateProjectDto
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
