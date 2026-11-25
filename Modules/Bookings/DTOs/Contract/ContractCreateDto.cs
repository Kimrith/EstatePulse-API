using System.ComponentModel.DataAnnotations;

namespace EstatePulse.Modules.Bookings.DTOs.Contract;

public sealed class ContractCreateDto
{
    [Required]
    public int BookingId { get; set; }

    [MaxLength(100)]
    public string? ContractNumber { get; set; }

    [MaxLength(500)]
    public string? DocumentUrl { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }
}
