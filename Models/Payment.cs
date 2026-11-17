using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Models;

[Table("Payments")]
public class Payment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("payment_reference")]
    public string PaymentReference { get; set; } = string.Empty;

    [Required]
    [Column("booking_id")]
    public int BookingId { get; set; }

    [Required]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Required]
    [Column("amount", TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(10)]
    [Column("currency")]
    public string Currency { get; set; } = "USD";

    [Required]
    [MaxLength(50)]
    [Column("payment_channel")]
    public string PaymentChannel { get; set; } = string.Empty; // e.g. BankTransfer, KHQR, CreditCard, Cash

    [MaxLength(255)]
    [Column("transaction_hash")]
    public string? TransactionHash { get; set; }

    [MaxLength(500)]
    [Column("proof_slip_url")]
    public string? ProofSlipUrl { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "Pending";

    [Column("verified_by_user_id")]
    public int? VerifiedByUserId { get; set; }

    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey(nameof(BookingId))]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey(nameof(CustomerId))]
    public virtual User Customer { get; set; } = null!;

    [ForeignKey(nameof(VerifiedByUserId))]
    public virtual User? VerifiedByUser { get; set; }

    public virtual ICollection<InstallmentSchedule> InstallmentSchedules { get; set; } = new List<InstallmentSchedule>();
}

