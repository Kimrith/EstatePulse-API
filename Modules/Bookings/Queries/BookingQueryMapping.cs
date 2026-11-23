using System.Data.Common;
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
            AgentId = reader.IsDBNull(offset + 4) ? null : reader.GetInt32(offset + 4),
            AgreedPrice = reader.GetDecimal(offset + 5),
            DiscountAmount = reader.GetDecimal(offset + 6),
            DepositRequired = reader.GetDecimal(offset + 7),
            Status = reader.GetString(offset + 8),
            ReservationExpiresAt = reader.IsDBNull(offset + 9) ? null : reader.GetDateTime(offset + 9),
            Notes = reader.IsDBNull(offset + 10) ? null : reader.GetString(offset + 10),
            CreatedAt = reader.GetDateTime(offset + 11),
            UpdatedAt = reader.IsDBNull(offset + 12) ? null : reader.GetDateTime(offset + 12),
            UnitCode = reader.IsDBNull(offset + 13) ? null : reader.GetString(offset + 13),
            CustomerName = reader.IsDBNull(offset + 14) ? null : reader.GetString(offset + 14),
            CustomerEmail = reader.IsDBNull(offset + 15) ? null : reader.GetString(offset + 15),
            CustomerPhone = reader.IsDBNull(offset + 16) ? null : reader.GetString(offset + 16),
            AgentCode = reader.IsDBNull(offset + 17) ? null : reader.GetString(offset + 17)
        };
    }
}
