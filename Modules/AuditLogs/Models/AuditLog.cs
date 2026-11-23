using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Modules.AuditLogs.Models;

[Table("AuditLogs")]
public class AuditLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("action")]
    public string Action { get; set; } = string.Empty; // e.g. CREATE, UPDATE, DELETE, LOGIN

    [Required]
    [MaxLength(100)]
    [Column("entity_name")]
    public string EntityName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("entity_id")]
    public string EntityId { get; set; } = string.Empty;

    [Column("old_values_json")]
    public string? OldValuesJson { get; set; }

    [Column("new_values_json")]
    public string? NewValuesJson { get; set; }

    [MaxLength(45)]
    [Column("ip_address")]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    [Column("user_agent")]
    public string? UserAgent { get; set; }

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
