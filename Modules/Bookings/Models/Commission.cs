using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Modules.Bookings.Models;

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
    [Column("commission_rate", TypeName = "decimal(5, 2)")]
    public decimal CommissionRate { get; set; }

    [Required]
    [Column("commission_amount", TypeName = "decimal(18, 2)")]
    public decimal CommissionAmount { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "Pending"; // e.g. Pending, Approved, Paid, Cancelled

    [Column("approved_by_user_id")]
    public int? ApprovedByUserId { get; set; }

    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
