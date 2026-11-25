using System.ComponentModel.DataAnnotations;

namespace EstatePulse.Modules.Bookings.DTOs.Commissions;

public sealed class CommissionUpdateDto
{
    [Range(0, 100, ErrorMessage = "Commission rate must be between 0 and 100.")]
    public decimal? CommissionRate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Commission amount must be a non-negative amount.")]
    public decimal? CommissionAmount { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public int? ApprovedByUserId { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? PaidAt { get; set; }
}
