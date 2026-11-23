using System.ComponentModel.DataAnnotations;

namespace EstatePulse.Modules.Units.DTOs;

public sealed class CreateUnitDto
{
    [Required]
    public int PhaseId { get; set; }

    [Required]
    public int UnitTypeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string UnitCode { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? StreetNumber { get; set; }

    [Required]
    [Range(1, 100000)]
    public decimal LandAreaSqm { get; set; }

    [Required]
    [Range(1, 100000)]
    public decimal BuiltAreaSqm { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal BasePrice { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Available";

    [MaxLength(100)]
    public string? SvgElementId { get; set; }
}
