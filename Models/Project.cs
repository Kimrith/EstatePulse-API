using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Models;

[Table("Projects")]
public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("location_address")]
    public string? LocationAddress { get; set; }

    [MaxLength(100)]
    [Column("city")]
    public string? City { get; set; }

    [Column("latitude", TypeName = "decimal(10, 8)")]
    public decimal? Latitude { get; set; }

    [Column("longitude", TypeName = "decimal(11, 8)")]
    public decimal? Longitude { get; set; }

    [MaxLength(500)]
    [Column("thumbnail_url")]
    public string? ThumbnailUrl { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Required]
    [Column("is_published")]
    public bool IsPublished { get; set; } = false;

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ICollection<ProjectPhase> Phases { get; set; } = new List<ProjectPhase>();
}

