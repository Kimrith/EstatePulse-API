using System.Data;
using System.Text;
using EstatePulse.Common.Database;
using EstatePulse.Common.Results;
using EstatePulse.Modules.Bookings.DTOs.Contract;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed class GetPaginatedContractsQueryHandler : IRequestHandler<GetPaginatedContractsQuery, PaginatedList<ContractDto>>
{
    private readonly AppDbContext _context;

    public GetPaginatedContractsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<PaginatedList<ContractDto>> Handle(GetPaginatedContractsQuery request, CancellationToken cancellationToken)
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

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            whereClauses.Add("c.[status] = @status");
            parameters.Add(new SqlParameter("@status", SqlDbType.NVarChar, 50) { Value = request.Status.Trim() });
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            whereClauses.Add("(c.[contract_number] LIKE @searchPattern OR b.[booking_reference] LIKE @searchPattern)");
            parameters.Add(new SqlParameter("@searchPattern", SqlDbType.NVarChar, 150) { Value = $"%{request.SearchTerm.Trim()}%" });
        }

        var whereSql = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : string.Empty;

        var sqlBuilder = new StringBuilder();
        sqlBuilder.Append(@"
            SELECT
                COUNT(*) OVER() AS [TotalCount],
                c.[id],
                c.[booking_id],
                c.[contract_number],
                c.[document_url],
                c.[status],
                c.[signed_at],
                c.[created_at],
                b.[booking_reference],
                u.[unit_code],
                usr.[full_name] AS [customer_name]
            FROM [Contracts] c
            INNER JOIN [Bookings] b ON c.[booking_id] = b.[id]
            INNER JOIN [Units] u ON b.[unit_id] = u.[id]
            INNER JOIN [Users] usr ON b.[customer_id] = usr.[id]
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
        var items = new List<ContractDto>(pageSize);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            if (totalCount == 0)
            {
                totalCount = reader.GetInt32(0);
            }

            items.Add(ContractQueryMapping.ReadContractDto(reader, 1));
        }

        return new PaginatedList<ContractDto>(items, totalCount, pageNumber, pageSize);
    }
}
