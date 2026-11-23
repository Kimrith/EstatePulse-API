using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Modules.Bookings.Models;

[Table("Contracts")]
public class Contract
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("booking_id")]
    public int BookingId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("contract_number")]
    public string ContractNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("document_url")]
    public string? DocumentUrl { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "Draft"; // e.g. Draft, Sent, Signed, Cancelled

    [Column("signed_at")]
    public DateTime? SignedAt { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
