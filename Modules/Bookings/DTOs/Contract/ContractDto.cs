namespace EstatePulse.Modules.Bookings.DTOs.Contract;

public sealed class ContractDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string? DocumentUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SignedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Joined view-model properties
    public string? BookingReference { get; set; }
    public string? UnitCode { get; set; }
    public string? CustomerName { get; set; }
}
