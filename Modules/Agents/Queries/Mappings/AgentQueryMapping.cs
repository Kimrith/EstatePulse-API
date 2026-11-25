using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Agents.DTOs.Agent;

namespace EstatePulse.Modules.Agents.Queries;

internal static class AgentQueryMapping
{
    public static AgentDto ReadAgentDto(DbDataReader reader, int offset = 0)
    {
        return new AgentDto
        {
            Id = reader.GetInt32(offset),
            UserId = reader.GetInt32(offset + 1),
            AgentCode = reader.GetString(offset + 2),
            LicenseNumber = reader.GetNullableString(offset + 3),
            CommissionRate = reader.GetDecimal(offset + 4),
            AssignedBranch = reader.GetNullableString(offset + 5),
            TotalDealsClosed = reader.GetInt32(offset + 6),
            Rating = reader.GetNullableDecimal(offset + 7),
            Bio = reader.GetNullableString(offset + 8),
            IsVerified = reader.GetBoolean(offset + 9),
            CreatedAt = reader.GetDateTime(offset + 10),
            UpdatedAt = reader.GetNullableDateTime(offset + 11)
        };
    }
}
