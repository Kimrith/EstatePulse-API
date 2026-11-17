using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Models;

[Table("Commissions")]
public class Commission
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("booking_id")]
    public int BookingId { get; set; }

    [Required]
    [Column("agent_id")]
    public int AgentId { get; set; }

    [Required]
    [Column("commission_percentage", TypeName = "decimal(5, 2)")]
    public decimal CommissionPercentage { get; set; }

    [Required]
    [Column("total_commission_amount", TypeName = "decimal(18, 2)")]
    public decimal TotalCommissionAmount { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("payment_status")]
    public string PaymentStatus { get; set; } = "Pending";

    [Column("approved_by_user_id")]
    public int? ApprovedByUserId { get; set; }

    [Column("disbursed_at")]
    public DateTime? DisbursedAt { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(BookingId))]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey(nameof(AgentId))]
    public virtual Agent Agent { get; set; } = null!;

    [ForeignKey(nameof(ApprovedByUserId))]
    public virtual User? ApprovedByUser { get; set; }
}

