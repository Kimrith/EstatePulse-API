using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Modules.Payments.Models;

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
    [Column("installment_number")]
    public int InstallmentNumber { get; set; }

    [Required]
    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Required]
    [Column("amount_due", TypeName = "decimal(18, 2)")]
    public decimal AmountDue { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "Pending"; // e.g. Pending, Paid, Overdue

    [Column("paid_date")]
    public DateTime? PaidDate { get; set; }

    [Column("payment_id")]
    public int? PaymentId { get; set; }

    [Column("late_fine_amount", TypeName = "decimal(18, 2)")]
    public decimal LateFineAmount { get; set; } = 0;
}
