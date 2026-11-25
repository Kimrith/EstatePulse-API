using System.ComponentModel.DataAnnotations;

namespace EstatePulse.Modules.Bookings.DTOs.Commissions;

public sealed class CommissionStatusUpdateDto
{
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    public int? ApprovedByUserId { get; set; }
}
