namespace EstatePulse.DTOs
{
    public class AgentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AgentCode { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public decimal CommissionRate { get; set; }
        public string? AssignedBranch { get; set; }
        public int TotalDealsClosed { get; set; } = 0;
        public decimal? Rating { get; set; }
        public string? Bio { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AgentCreateDto
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

    public class AgentUpdateDto
    {
        public string? LicenseNumber { get; set; }
        public decimal? CommissionRate { get; set; }
        public string? AssignedBranch { get; set; }
        public int? TotalDealsClosed { get; set; }
        public decimal? Rating { get; set; }
        public string? Bio { get; set; }
        public bool? IsVerified { get; set; }
    }
}