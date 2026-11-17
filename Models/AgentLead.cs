using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Models;

/// <summary>
/// Built-in CRM tracker for prospective buyers before they commit to an actual deal.
/// Tracks customer budget ranges, lead pipeline stages, contact information, and negotiation notes.
/// </summary>
[Table("AgentLeads")]
public class AgentLead
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// The selling agent managing this lead.
    /// </summary>
    [Required]
    [Column("agent_id")]
    public int AgentId { get; set; }

    /// <summary>
    /// Optional link to a registered User once the lead registers or is converted.
    /// </summary>
    [Column("customer_id")]
    public int? CustomerId { get; set; }

    /// <summary>
    /// Full name of the prospective buyer.
    /// </summary>
    [Required]
    [MaxLength(150)]
    [Column("lead_name")]
    public string LeadName { get; set; } = string.Empty;

    /// <summary>
    /// Primary contact phone number.
    /// </summary>
    [Required]
    [Phone]
    [MaxLength(30)]
    [Column("lead_phone")]
    public string LeadPhone { get; set; } = string.Empty;

    /// <summary>
    /// Optional email address of the prospective buyer.
    /// </summary>
    [MaxLength(255)]
    [EmailAddress]
    [Column("lead_email")]
    public string? LeadEmail { get; set; }

    /// <summary>
    /// Minimum target budget in USD.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Budget minimum must be a non-negative amount.")]
    [Column("budget_min", TypeName = "decimal(18, 2)")]
    public decimal? BudgetMin { get; set; }

    /// <summary>
    /// Maximum target budget in USD.
    /// </summary>
    [Range(0, double.MaxValue, ErrorMessage = "Budget maximum must be a non-negative amount.")]
    [Column("budget_max", TypeName = "decimal(18, 2)")]
    public decimal? BudgetMax { get; set; }

    /// <summary>
    /// Current pipeline status (e.g., New, Contacted, Qualified, Negotiation, Won, Lost).
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = Statuses.New;

    /// <summary>
    /// Meeting notes, customer preferences, unit requirements, and negotiation details.
    /// </summary>
    [MaxLength(2000)]
    [Column("notes")]
    public string? Notes { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey(nameof(AgentId))]
    public virtual Agent Agent { get; set; } = null!;

    [ForeignKey(nameof(CustomerId))]
    public virtual User? Customer { get; set; }

    // CRM Pipeline Status Constants
    public static class Statuses
    {
        public const string New = "New";
        public const string Contacted = "Contacted";
        public const string Qualified = "Qualified";
        public const string ProposalSent = "ProposalSent";
        public const string Negotiation = "Negotiation";
        public const string Won = "Won";
        public const string Lost = "Lost";
    }

    // Business Logic Helpers
    [NotMapped]
    public bool HasCustomerAccount => CustomerId.HasValue;

    [NotMapped]
    public bool IsWon => string.Equals(Status, Statuses.Won, StringComparison.OrdinalIgnoreCase);

    [NotMapped]
    public bool IsClosed => IsWon || string.Equals(Status, Statuses.Lost, StringComparison.OrdinalIgnoreCase);
}


