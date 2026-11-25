using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Contract;

namespace EstatePulse.Modules.Bookings.Queries;

internal static class ContractQueryMapping
{
    public static ContractDto ReadContractDto(DbDataReader reader, int offset = 0)
    {
        return new ContractDto
        {
            Id = reader.GetInt32(offset),
            BookingId = reader.GetInt32(offset + 1),
            ContractNumber = reader.GetString(offset + 2),
            DocumentUrl = reader.GetNullableString(offset + 3),
            Status = reader.GetString(offset + 4),
            SignedAt = reader.GetNullableDateTime(offset + 5),
            CreatedAt = reader.GetDateTime(offset + 6),
            BookingReference = reader.GetNullableString(offset + 7),
            UnitCode = reader.GetNullableString(offset + 8),
            CustomerName = reader.GetNullableString(offset + 9)
        };
    }
}
