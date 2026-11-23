namespace EstatePulse.Modules.Units.DTOs;

public sealed class UnitTypeDto
{
    public int Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int ParkingSlots { get; set; }
    public int Floors { get; set; }
    public decimal BuildingSize { get; set; }
    public decimal? LandSizeRecommended { get; set; }
    public string? FloorPlanImageUrl { get; set; }
    public string? SpecificationJson { get; set; }
    public int TotalUnitsCount { get; set; }
}
