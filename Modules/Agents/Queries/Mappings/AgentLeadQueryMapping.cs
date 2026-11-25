using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Agents.DTOs.AgentLead;

namespace EstatePulse.Modules.Agents.Queries;

internal static class AgentLeadQueryMapping
{
    public static AgentLeadDto ReadAgentLeadDto(DbDataReader reader, int offset = 0)
    {
        return new AgentLeadDto
        {
            Id = reader.GetInt32(offset),
            AgentId = reader.GetInt32(offset + 1),
            CustomerId = reader.GetNullableInt32(offset + 2),
            LeadName = reader.GetString(offset + 3),
            LeadPhone = reader.GetString(offset + 4),
            LeadEmail = reader.GetNullableString(offset + 5),
            BudgetMin = reader.GetNullableDecimal(offset + 6),
            BudgetMax = reader.GetNullableDecimal(offset + 7),
            Status = reader.GetString(offset + 8),
            Notes = reader.GetNullableString(offset + 9),
            CreatedAt = reader.GetDateTime(offset + 10),
            UpdatedAt = reader.GetNullableDateTime(offset + 11),
            AgentCode = reader.GetNullableString(offset + 12),
            CustomerName = reader.GetNullableString(offset + 13)
        };
    }
}
