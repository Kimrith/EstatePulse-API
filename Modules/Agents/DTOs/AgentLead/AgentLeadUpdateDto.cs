namespace EstatePulse.Modules.Agents.DTOs.AgentLead;

public sealed class AgentLeadUpdateDto
{
    public int? CustomerId { get; set; }
    public string? LeadName { get; set; }
    public string? LeadPhone { get; set; }
    public string? LeadEmail { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}
