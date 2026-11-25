using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Booking;

namespace EstatePulse.Modules.Bookings.Queries;

internal static class BookingQueryMapping
{
    public static BookingDto ReadBookingDto(DbDataReader reader, int offset = 0)
    {
        return new BookingDto
        {
            Id = reader.GetInt32(offset),
            BookingReference = reader.GetString(offset + 1),
            UnitId = reader.GetInt32(offset + 2),
            CustomerId = reader.GetInt32(offset + 3),
            AgentId = reader.GetNullableInt32(offset + 4),
            AgreedPrice = reader.GetDecimal(offset + 5),
            DiscountAmount = reader.GetDecimal(offset + 6),
            DepositRequired = reader.GetDecimal(offset + 7),
            Status = reader.GetString(offset + 8),
            ReservationExpiresAt = reader.GetNullableDateTime(offset + 9),
            Notes = reader.GetNullableString(offset + 10),
            CreatedAt = reader.GetDateTime(offset + 11),
            UpdatedAt = reader.GetNullableDateTime(offset + 12),
            UnitCode = reader.GetNullableString(offset + 13),
            CustomerName = reader.GetNullableString(offset + 14),
            CustomerEmail = reader.GetNullableString(offset + 15),
            CustomerPhone = reader.GetNullableString(offset + 16),
            AgentCode = reader.GetNullableString(offset + 17)
        };
    }
}
