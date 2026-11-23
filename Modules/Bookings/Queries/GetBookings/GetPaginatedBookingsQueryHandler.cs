using System.Data;
using System.Text;
using EstatePulse.Common.Database;
using EstatePulse.Common.Results;
using EstatePulse.Modules.Bookings.DTOs.Booking;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed class GetPaginatedBookingsQueryHandler : IRequestHandler<GetPaginatedBookingsQuery, PaginatedList<BookingDto>>
{
    private readonly AppDbContext _context;

    public GetPaginatedBookingsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<PaginatedList<BookingDto>> Handle(GetPaginatedBookingsQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        int pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        int offset = (pageNumber - 1) * pageSize;

        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        var whereClauses = new List<string>();
        var parameters = new List<SqlParameter>();

        if (request.UnitId.HasValue)
        {
            whereClauses.Add("b.[unit_id] = @unitId");
            parameters.Add(new SqlParameter("@unitId", SqlDbType.Int) { Value = request.UnitId.Value });
        }

        if (request.CustomerId.HasValue)
        {
            whereClauses.Add("b.[customer_id] = @customerId");
            parameters.Add(new SqlParameter("@customerId", SqlDbType.Int) { Value = request.CustomerId.Value });
        }

        if (request.AgentId.HasValue)
        {
            whereClauses.Add("b.[agent_id] = @agentId");
            parameters.Add(new SqlParameter("@agentId", SqlDbType.Int) { Value = request.AgentId.Value });
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            whereClauses.Add("b.[status] = @status");
            parameters.Add(new SqlParameter("@status", SqlDbType.NVarChar, 50) { Value = request.Status.Trim() });
        }

        var whereSql = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : string.Empty;

        var sqlBuilder = new StringBuilder();
        sqlBuilder.Append(@"
            SELECT 
                COUNT(*) OVER() AS [TotalCount],
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
        ");

        if (!string.IsNullOrEmpty(whereSql))
        {
            sqlBuilder.AppendLine(whereSql);
        }

        sqlBuilder.AppendLine("ORDER BY b.[created_at] DESC");
        sqlBuilder.AppendLine("OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;");

        await using var command = connection.CreateCommand();
        command.CommandText = sqlBuilder.ToString();

        foreach (var p in parameters)
        {
            command.Parameters.Add(p);
        }

        command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
        command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

        int totalCount = 0;
        var items = new List<BookingDto>(pageSize);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            if (totalCount == 0)
            {
                totalCount = reader.GetInt32(0);
            }

            items.Add(BookingQueryMapping.ReadBookingDto(reader, 1));
        }

        if (totalCount == 0 && offset > 0)
        {
            await using var countCmd = connection.CreateCommand();
            var countSql = "SELECT COUNT(1) FROM [Bookings] b " + whereSql;
            countCmd.CommandText = countSql;
            foreach (var p in parameters)
            {
                var clone = new SqlParameter(p.ParameterName, p.SqlDbType, p.Size) { Value = p.Value };
                countCmd.Parameters.Add(clone);
            }
            var scalar = await countCmd.ExecuteScalarAsync(cancellationToken);
            totalCount = Convert.ToInt32(scalar);
        }

        return new PaginatedList<BookingDto>(items, totalCount, pageNumber, pageSize);
    }
}
