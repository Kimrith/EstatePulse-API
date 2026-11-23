using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Modules.Agents.Models;

[Table("Agents")]
public class Agent
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("agent_code")]
    public string AgentCode { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("license_number")]
    public string? LicenseNumber { get; set; }

    [Required]
    [Column("commission_rate", TypeName = "decimal(5, 2)")]
    public decimal CommissionRate { get; set; }

    [MaxLength(150)]
    [Column("assigned_branch")]
    public string? AssignedBranch { get; set; }

    [Required]
    [Column("total_deals_closed")]
    public int TotalDealsClosed { get; set; } = 0;

    [Column("rating", TypeName = "decimal(3, 2)")]
    public decimal? Rating { get; set; }

    [MaxLength(1000)]
    [Column("bio")]
    public string? Bio { get; set; }

    [Required]
    [Column("is_verified")]
    public bool IsVerified { get; set; } = false;

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
