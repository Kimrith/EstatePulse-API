namespace EstatePulse.Modules.Units.DTOs;

public sealed class UnitMediaDto
{
    public int Id { get; set; }
    public int? UnitId { get; set; }
    public string? UnitCode { get; set; }
    public int? UnitTypeId { get; set; }
    public string? UnitTypeCode { get; set; }
    public string MediaType { get; set; } = "image";
    public string MediaUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsFeatured { get; set; }
}
