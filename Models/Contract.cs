using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Models;

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
    [Column("generated_pdf_url")]
    public string? GeneratedPdfUrl { get; set; }

    [MaxLength(500)]
    [Column("signed_pdf_url")]
    public string? SignedPdfUrl { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "Draft"; // e.g. Draft, Sent, Signed, Expired, Void

    [Column("signing_deadline")]
    public DateTime? SigningDeadline { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(BookingId))]
    public virtual Booking Booking { get; set; } = null!;
}

