using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Units.DTOs;

public sealed class CreateUnitTypeDto
{
    [Required]
    [MaxLength(100)]
    public string TypeName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Range(0, 100)]
    public int Bedrooms { get; set; }

    [Required]
    [Range(0, 100)]
    public int Bathrooms { get; set; }

    [Required]
    [Range(0, 100)]
    public int ParkingSlots { get; set; }

    [Required]
    [Range(1, 200)]
    public int Floors { get; set; } = 1;

    [Required]
    [Range(1, 100000)]
    public decimal BuildingSize { get; set; }

    public decimal? LandSizeRecommended { get; set; }

    public IFormFile? FloorPlanImageFile { get; set; }

    public string? SpecificationJson { get; set; }
}
