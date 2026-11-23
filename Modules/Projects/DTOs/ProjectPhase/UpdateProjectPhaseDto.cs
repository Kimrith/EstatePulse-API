using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EstatePulse.Modules.Projects.DTOs;

public sealed class UpdateProjectPhaseDto
{
    public int? ProjectId { get; set; }

    [MaxLength(150)]
    public string? PhaseName { get; set; }

    [MaxLength(150)]
    public string? Slug { get; set; }

    public IFormFile? MasterPlanSvgFile { get; set; }

    public DateTime? LaunchDate { get; set; }

    public DateTime? ExpectedCompletionDate { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }
}
