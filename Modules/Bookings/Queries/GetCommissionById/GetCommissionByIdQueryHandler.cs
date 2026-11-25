using System.Data;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Commissions;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed class GetCommissionByIdQueryHandler : IRequestHandler<GetCommissionByIdQuery, CommissionDto?>
{
    private readonly AppDbContext _context;

    public GetCommissionByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<CommissionDto?> Handle(GetCommissionByIdQuery request, CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT TOP(1)
                c.[id],
                c.[booking_id],
                c.[agent_id],
                c.[commission_rate],
                c.[commission_amount],
                c.[status],
                c.[approved_by_user_id],
                c.[approved_at],
                c.[paid_at],
                c.[created_at],
                b.[booking_reference],
                a.[agent_code],
                u.[full_name] AS [approved_by_user_name]
            FROM [Commissions] c
            INNER JOIN [Bookings] b ON c.[booking_id] = b.[id]
            INNER JOIN [Agents] a ON c.[agent_id] = a.[id]
            LEFT JOIN [Users] u ON c.[approved_by_user_id] = u.[id]
            WHERE c.[id] = @id;";

        command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = request.Id });

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return CommissionQueryMapping.ReadCommissionDto(reader);
        }

        return null;
    }
}
