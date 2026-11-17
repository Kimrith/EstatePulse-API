using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Models;

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
    public string MediaType { get; set; } = "image"; // e.g. image, video, 360_tour, floor_plan

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

    // Navigation Properties
    [ForeignKey(nameof(UnitId))]
    public virtual Unit? Unit { get; set; }

    [ForeignKey(nameof(UnitTypeId))]
    public virtual UnitType? UnitType { get; set; }
}

