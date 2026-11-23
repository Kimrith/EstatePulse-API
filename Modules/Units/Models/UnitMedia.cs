using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Modules.Units.Models;

[Table("UnitMedia")]
public class UnitMedia
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("unit_id")]
    public int? UnitId { get; set; }

    [Column("unit_type_id")]
    public int? UnitTypeId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("media_type")]
    public string MediaType { get; set; } = "image"; // e.g. image, video, 3d_tour, floorplan

    [Required]
    [MaxLength(500)]
    [Column("media_url")]
    public string MediaUrl { get; set; } = string.Empty;

    [Required]
    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Required]
    [Column("is_featured")]
    public bool IsFeatured { get; set; } = false;

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
