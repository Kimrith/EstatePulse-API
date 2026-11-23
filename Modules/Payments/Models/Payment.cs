using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Modules.Payments.Models;

[Table("Payments")]
public class Payment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
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
    [MaxLength(50)]
    [Column("payment_method")]
    public string PaymentMethod { get; set; } = "BakongKHQR"; // e.g. BakongKHQR, BankTransfer, Cash, CreditCard

    [MaxLength(100)]
    [Column("transaction_id")]
    public string? TransactionId { get; set; }

    [MaxLength(500)]
    [Column("receipt_image_url")]
    public string? ReceiptImageUrl { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "Pending"; // e.g. Pending, Completed, Failed, Verified

    [Column("verified_by_user_id")]
    public int? VerifiedByUserId { get; set; }

    [Column("verified_at")]
    public DateTime? VerifiedAt { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
