using System.ComponentModel.DataAnnotations;

namespace EstatePulse.Modules.Bookings.DTOs.Booking;

public sealed class BookingUpdateDto
{
    public int? AgentId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Agreed price must be a non-negative amount.")]
    public decimal? AgreedPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Discount amount must be a non-negative amount.")]
    public decimal? DiscountAmount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Deposit required must be a non-negative amount.")]
    public decimal? DepositRequired { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public DateTime? ReservationExpiresAt { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}
