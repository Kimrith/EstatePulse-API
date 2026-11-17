using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Models;

[Table("InstallmentSchedules")]
public class InstallmentSchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("booking_id")]
    public int BookingId { get; set; }

    [Required]
    [Column("stage_number")]
    public int StageNumber { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("milestone_title")]
    public string MilestoneTitle { get; set; } = string.Empty;

    [Required]
    [Column("due_amount", TypeName = "decimal(18, 2)")]
    public decimal DueAmount { get; set; }

    [Required]
    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Required]
    [Column("is_paid")]
    public bool IsPaid { get; set; } = false;

    [Column("payment_id")]
    public int? PaymentId { get; set; }

    [Column("paid_at")]
    public DateTime? PaidAt { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(BookingId))]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey(nameof(PaymentId))]
    public virtual Payment? Payment { get; set; }
}

