namespace EstatePulse.Modules.Bookings.DTOs.Commissions;

public sealed class CommissionDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int AgentId { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Joined view-model properties
    public string? BookingReference { get; set; }
    public string? AgentCode { get; set; }
    public string? ApprovedByUserName { get; set; }
}
