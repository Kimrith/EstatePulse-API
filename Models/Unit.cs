using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Models;

[Table("Units")]
public class Unit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("phase_id")]
    public int PhaseId { get; set; }

    [Required]
    [Column("unit_type_id")]
    public int UnitTypeId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("unit_code")]
    public string UnitCode { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("street_number")]
    public string? StreetNumber { get; set; }

    [Required]
    [Column("land_area_sqm", TypeName = "decimal(10, 2)")]
    public decimal LandAreaSqm { get; set; }

    [Required]
    [Column("built_area_sqm", TypeName = "decimal(10, 2)")]
    public decimal BuiltAreaSqm { get; set; }

    [Required]
    [Column("base_price", TypeName = "decimal(18, 2)")]
    public decimal BasePrice { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "Available";

    [Column("hold_expires_at")]
    public DateTime? HoldExpiresAt { get; set; }

    [Column("current_hold_user_id")]
    public int? CurrentHoldUserId { get; set; }

    [MaxLength(100)]
    [Column("svg_element_id")]
    public string? SvgElementId { get; set; }

    [Timestamp]
    [Column("row_version")]
    public byte[]? RowVersion { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(PhaseId))]
    public virtual ProjectPhase Phase { get; set; } = null!;

    [ForeignKey(nameof(UnitTypeId))]
    public virtual UnitType UnitType { get; set; } = null!;

    [ForeignKey(nameof(CurrentHoldUserId))]
    public virtual User? CurrentHoldUser { get; set; }

    public virtual ICollection<UnitMedia> Media { get; set; } = new List<UnitMedia>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}

