namespace EstatePulse.Modules.Projects.DTOs;

public sealed class ProjectPhaseDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string PhaseName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? MasterPlanSvgUrl { get; set; }
    public DateTime? LaunchDate { get; set; }
    public DateTime? ExpectedCompletionDate { get; set; }
    public string Status { get; set; } = "Planning";
    public DateTime CreatedAt { get; set; }
    public string? ProjectName { get; set; }
}
