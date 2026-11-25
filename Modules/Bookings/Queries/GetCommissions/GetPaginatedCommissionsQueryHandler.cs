using System.Data;
using System.Text;
using EstatePulse.Common.Database;
using EstatePulse.Common.Results;
using EstatePulse.Modules.Bookings.DTOs.Commissions;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed class GetPaginatedCommissionsQueryHandler : IRequestHandler<GetPaginatedCommissionsQuery, PaginatedList<CommissionDto>>
{
    private readonly AppDbContext _context;

    public GetPaginatedCommissionsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<PaginatedList<CommissionDto>> Handle(GetPaginatedCommissionsQuery request, CancellationToken cancellationToken)
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

        if (request.BookingId.HasValue)
        {
            whereClauses.Add("c.[booking_id] = @bookingId");
            parameters.Add(new SqlParameter("@bookingId", SqlDbType.Int) { Value = request.BookingId.Value });
        }

        if (request.AgentId.HasValue)
        {
            whereClauses.Add("c.[agent_id] = @agentId");
            parameters.Add(new SqlParameter("@agentId", SqlDbType.Int) { Value = request.AgentId.Value });
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            whereClauses.Add("c.[status] = @status");
            parameters.Add(new SqlParameter("@status", SqlDbType.NVarChar, 50) { Value = request.Status.Trim() });
        }

        var whereSql = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : string.Empty;

        var sqlBuilder = new StringBuilder();
        sqlBuilder.Append(@"
            SELECT
                COUNT(*) OVER() AS [TotalCount],
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
        ");

        if (!string.IsNullOrEmpty(whereSql))
        {
            sqlBuilder.AppendLine(whereSql);
        }

        sqlBuilder.AppendLine("ORDER BY c.[created_at] DESC, c.[id] DESC");
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
        var items = new List<CommissionDto>(pageSize);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            if (totalCount == 0)
            {
                totalCount = reader.GetInt32(0);
            }

            items.Add(CommissionQueryMapping.ReadCommissionDto(reader, 1));
        }

        return new PaginatedList<CommissionDto>(items, totalCount, pageNumber, pageSize);
    }
}
