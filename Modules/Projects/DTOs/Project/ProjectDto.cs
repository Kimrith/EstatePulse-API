namespace EstatePulse.Modules.Projects.DTOs;

public sealed class ProjectDto
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
