using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Units.DTOs;

public sealed class CreateUnitMediaDto
{
    public int? UnitId { get; set; }
    public int? UnitTypeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string MediaType { get; set; } = "image";

    public IFormFile? MediaFile { get; set; }

    [MaxLength(500)]
    public string? MediaUrl { get; set; }

    public int SortOrder { get; set; } = 0;
    public bool IsFeatured { get; set; } = false;
}
