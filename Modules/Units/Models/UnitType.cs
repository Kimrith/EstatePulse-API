using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Modules.Units.Models;

[Table("UnitTypes")]
public class UnitType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("type_name")]
    public string TypeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Column("bedrooms")]
    public int Bedrooms { get; set; }

    [Required]
    [Column("bathrooms")]
    public int Bathrooms { get; set; }

    [Required]
    [Column("parking_slots")]
    public int ParkingSlots { get; set; }

    [Required]
    [Column("floors")]
    public int Floors { get; set; } = 1;

    [Required]
    [Column("building_size", TypeName = "decimal(10, 2)")]
    public decimal BuildingSize { get; set; }

    [Column("land_size_recommended", TypeName = "decimal(10, 2)")]
    public decimal? LandSizeRecommended { get; set; }

    [MaxLength(500)]
    [Column("floor_plan_image_url")]
    public string? FloorPlanImageUrl { get; set; }

    [Column("specification_json")]
    public string? SpecificationJson { get; set; }
}
