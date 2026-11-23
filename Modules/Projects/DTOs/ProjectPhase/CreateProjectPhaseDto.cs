using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Projects.DTOs;

public sealed class CreateProjectPhaseDto
{
    [Required]
    public int ProjectId { get; set; }

    [Required]
    [MaxLength(150)]
    public string PhaseName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Slug { get; set; } = string.Empty;

    public IFormFile? MasterPlanSvgFile { get; set; }

    public DateTime? LaunchDate { get; set; }

    public DateTime? ExpectedCompletionDate { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Planning";
}
