using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Commissions;

namespace EstatePulse.Modules.Bookings.Queries;

internal static class CommissionQueryMapping
{
    public static CommissionDto ReadCommissionDto(DbDataReader reader, int offset = 0)
    {
        return new CommissionDto
        {
            Id = reader.GetInt32(offset),
            BookingId = reader.GetInt32(offset + 1),
            AgentId = reader.GetInt32(offset + 2),
            CommissionRate = reader.GetDecimal(offset + 3),
            CommissionAmount = reader.GetDecimal(offset + 4),
            Status = reader.GetString(offset + 5),
            ApprovedByUserId = reader.GetNullableInt32(offset + 6),
            ApprovedAt = reader.GetNullableDateTime(offset + 7),
            PaidAt = reader.GetNullableDateTime(offset + 8),
            CreatedAt = reader.GetDateTime(offset + 9),
            BookingReference = reader.GetNullableString(offset + 10),
            AgentCode = reader.GetNullableString(offset + 11),
            ApprovedByUserName = reader.GetNullableString(offset + 12)
        };
    }
}
