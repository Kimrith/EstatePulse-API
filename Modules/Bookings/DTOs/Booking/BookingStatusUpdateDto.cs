using System.ComponentModel.DataAnnotations;

namespace EstatePulse.Modules.Bookings.DTOs.Booking;

public sealed class BookingStatusUpdateDto
{
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
}
