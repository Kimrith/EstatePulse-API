using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Units.DTOs;

public sealed class UpdateUnitTypeDto
{
    [MaxLength(100)]
    public string? TypeName { get; set; }

    [MaxLength(50)]
    public string? Code { get; set; }

    [Range(0, 100)]
    public int? Bedrooms { get; set; }

    [Range(0, 100)]
    public int? Bathrooms { get; set; }

    [Range(0, 100)]
    public int? ParkingSlots { get; set; }

    [Range(1, 200)]
    public int? Floors { get; set; }

    public decimal? BuildingSize { get; set; }

    public decimal? LandSizeRecommended { get; set; }

    public IFormFile? FloorPlanImageFile { get; set; }

    public string? SpecificationJson { get; set; }
}
