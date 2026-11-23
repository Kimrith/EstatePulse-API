using System.Data;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Booking;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDto?>
{
    private readonly AppDbContext _context;

    public GetBookingByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<BookingDto?> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                b.[id],
                b.[booking_reference],
                b.[unit_id],
                b.[customer_id],
                b.[agent_id],
                b.[agreed_price],
                b.[discount_amount],
                b.[deposit_required],
                b.[status],
                b.[reservation_expires_at],
                b.[notes],
                b.[created_at],
                b.[updated_at],
                u.[unit_code],
                usr.[full_name] AS [customer_name],
                usr.[email] AS [customer_email],
                usr.[phone_number] AS [customer_phone],
                a.[agent_code]
            FROM [Bookings] b
            INNER JOIN [Units] u ON b.[unit_id] = u.[id]
            INNER JOIN [Users] usr ON b.[customer_id] = usr.[id]
            LEFT JOIN [Agents] a ON b.[agent_id] = a.[id]
            WHERE b.[id] = @id;";

        command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = request.Id });

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return BookingQueryMapping.ReadBookingDto(reader, 0);
        }

        return null;
    }
}
