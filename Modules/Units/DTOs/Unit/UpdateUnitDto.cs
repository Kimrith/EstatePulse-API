using System.ComponentModel.DataAnnotations;

namespace EstatePulse.Modules.Units.DTOs;

public sealed class UpdateUnitDto
{
    public int? PhaseId { get; set; }
    public int? UnitTypeId { get; set; }

    [MaxLength(50)]
    public string? UnitCode { get; set; }

    [MaxLength(50)]
    public string? StreetNumber { get; set; }

    public decimal? LandAreaSqm { get; set; }
    public decimal? BuiltAreaSqm { get; set; }
    public decimal? BasePrice { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    [MaxLength(100)]
    public string? SvgElementId { get; set; }
}
