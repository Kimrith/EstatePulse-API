using System.ComponentModel.DataAnnotations;

namespace EstatePulse.Modules.Units.DTOs;

public sealed class HoldUnitDto
{
    [Required]
    public int UserId { get; set; }

    [Range(1, 120)]
    public int HoldMinutes { get; set; } = 15;

    /// <summary>
    /// Base64-encoded concurrency rowversion token retrieved from previous GET response.
    /// </summary>
    public string? RowVersion { get; set; }
}
