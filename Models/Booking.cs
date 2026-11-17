using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Models;

[Table("Bookings")]
public class Booking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("booking_reference")]
    public string BookingReference { get; set; } = string.Empty;

    [Required]
    [Column("unit_id")]
    public int UnitId { get; set; }

    [Required]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Column("agent_id")]
    public int? AgentId { get; set; }

    [Required]
    [Column("agreed_price", TypeName = "decimal(18, 2)")]
    public decimal AgreedPrice { get; set; }

    [Required]
    [Column("discount_amount", TypeName = "decimal(18, 2)")]
    public decimal DiscountAmount { get; set; } = 0m;

    [Required]
    [Column("deposit_required", TypeName = "decimal(18, 2)")]
    public decimal DepositRequired { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "Pending";

    [Column("reservation_expires_at")]
    public DateTime? ReservationExpiresAt { get; set; }

    [MaxLength(2000)]
    [Column("notes")]
    public string? Notes { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(UnitId))]
    public virtual Unit Unit { get; set; } = null!;

    [ForeignKey(nameof(CustomerId))]
    public virtual User Customer { get; set; } = null!;

    [ForeignKey(nameof(AgentId))]
    public virtual Agent? Agent { get; set; }

    public virtual ICollection<Commission> Commissions { get; set; } = new List<Commission>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<InstallmentSchedule> InstallmentSchedules { get; set; } = new List<InstallmentSchedule>();
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}

