namespace EstatePulse.Modules.Units.DTOs;

public sealed class UnitDto
{
    public int Id { get; set; }
    public int PhaseId { get; set; }
    public string? PhaseName { get; set; }
    public int? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public int UnitTypeId { get; set; }
    public string? UnitTypeCode { get; set; }
    public string? UnitTypeName { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string? StreetNumber { get; set; }
    public decimal LandAreaSqm { get; set; }
    public decimal BuiltAreaSqm { get; set; }
    public decimal BasePrice { get; set; }
    public string Status { get; set; } = "Available";
    public DateTime? HoldExpiresAt { get; set; }
    public int? CurrentHoldUserId { get; set; }
    public string? CurrentHoldUserName { get; set; }
    public int? HoldRemainingSeconds { get; set; }
    public string? SvgElementId { get; set; }
    public string? RowVersion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
