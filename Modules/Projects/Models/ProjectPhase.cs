using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstatePulse.Modules.Projects.Models;

[Table("ProjectPhases")]
public class ProjectPhase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("project_id")]
    public int ProjectId { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("phase_name")]
    public string PhaseName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    [Column("master_plan_svg_url")]
    public string? MasterPlanSvgUrl { get; set; }

    [Column("launch_date")]
    public DateTime? LaunchDate { get; set; }

    [Column("expected_completion_date")]
    public DateTime? ExpectedCompletionDate { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "Planning";

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
