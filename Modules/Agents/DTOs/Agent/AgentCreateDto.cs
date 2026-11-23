namespace EstatePulse.Modules.Agents.DTOs.Agent;

public sealed class AgentCreateDto
{
    public int UserId { get; set; }
    public string AgentCode { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public decimal CommissionRate { get; set; }
    public string? AssignedBranch { get; set; }
    public int TotalDealsClosed { get; set; } = 0;
    public decimal? Rating { get; set; }
    public string? Bio { get; set; }
    public bool IsVerified { get; set; } = false;
}
