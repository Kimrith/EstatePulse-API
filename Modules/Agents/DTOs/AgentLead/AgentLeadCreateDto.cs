namespace EstatePulse.Modules.Agents.DTOs.AgentLead;

public sealed class AgentLeadCreateDto
{
    public int AgentId { get; set; }
    public int? CustomerId { get; set; }
    public string LeadName { get; set; } = string.Empty;
    public string LeadPhone { get; set; } = string.Empty;
    public string? LeadEmail { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public string Status { get; set; } = "New";
    public string? Notes { get; set; }
}
