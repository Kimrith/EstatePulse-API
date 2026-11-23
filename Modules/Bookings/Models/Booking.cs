using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Modules.Bookings.Models;

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
    public decimal DiscountAmount { get; set; } = 0;

    [Required]
    [Column("deposit_required", TypeName = "decimal(18, 2)")]
    public decimal DepositRequired { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = Statuses.Draft;

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

    public static class Statuses
    {
        public const string Draft = "Draft";
        public const string Reserved = "Reserved";
        public const string Confirmed = "Confirmed";
        public const string Cancelled = "Cancelled";
        public const string Expired = "Expired";

        public static readonly HashSet<string> All = new(StringComparer.OrdinalIgnoreCase)
        {
            Draft, Reserved, Confirmed, Cancelled, Expired
        };
    }
}
