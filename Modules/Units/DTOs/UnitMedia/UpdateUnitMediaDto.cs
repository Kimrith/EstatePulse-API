using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Units.DTOs;

public sealed class UpdateUnitMediaDto
{
    public int? UnitId { get; set; }
    public int? UnitTypeId { get; set; }

    [MaxLength(50)]
    public string? MediaType { get; set; }

    public IFormFile? MediaFile { get; set; }

    [MaxLength(500)]
    public string? MediaUrl { get; set; }

    public int? SortOrder { get; set; }
    public bool? IsFeatured { get; set; }
}
