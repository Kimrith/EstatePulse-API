using System.ComponentModel.DataAnnotations;

namespace EstatePulse.Modules.Bookings.DTOs.Contract;

public sealed class ContractStatusUpdateDto
{
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
}
