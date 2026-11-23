namespace EstatePulse.Modules.Agents.DTOs.Agent;

public sealed class AgentUpdateDto
{
    public string? LicenseNumber { get; set; }
    public decimal? CommissionRate { get; set; }
    public string? AssignedBranch { get; set; }
    public int? TotalDealsClosed { get; set; }
    public decimal? Rating { get; set; }
    public string? Bio { get; set; }
    public bool? IsVerified { get; set; }
}
