namespace EstatePulse.Modules.Bookings.DTOs.Booking;

public sealed class BookingDto
{
    public int Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public int CustomerId { get; set; }
    public int? AgentId { get; set; }
    public decimal AgreedPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DepositRequired { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ReservationExpiresAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Joined view-model properties
    public string? UnitCode { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? AgentCode { get; set; }
}
