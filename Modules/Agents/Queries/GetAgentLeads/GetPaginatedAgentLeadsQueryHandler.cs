using System.Data;
using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Common.Results;
using EstatePulse.Modules.Agents.DTOs.AgentLead;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Agents.Queries;

public sealed class GetPaginatedAgentLeadsQueryHandler : IRequestHandler<GetPaginatedAgentLeadsQuery, PaginatedList<AgentLeadDto>>
{
    private readonly AppDbContext _context;

    public GetPaginatedAgentLeadsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<PaginatedList<AgentLeadDto>> Handle(GetPaginatedAgentLeadsQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        int pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        int offset = (pageNumber - 1) * pageSize;

        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();

        bool hasSearch = !string.IsNullOrWhiteSpace(request.SearchTerm);
        var searchPattern = hasSearch ? $"%{request.SearchTerm!.Trim()}%" : null;

        command.CommandText = @"
            SELECT 
                COUNT(*) OVER() AS [TotalCount],
                al.[id],
                al.[agent_id],
                al.[customer_id],
                al.[lead_name],
                al.[lead_phone],
                al.[lead_email],
                al.[budget_min],
                al.[budget_max],
                al.[status],
                al.[notes],
                al.[created_at],
                al.[updated_at],
                a.[agent_code],
                u.[full_name] AS [customer_name]
            FROM [AgentLeads] al
            INNER JOIN [Agents] a ON a.[id] = al.[agent_id]
            LEFT JOIN [Users] u ON u.[id] = al.[customer_id]
            WHERE (@agentId IS NULL OR al.[agent_id] = @agentId)
              AND (@status IS NULL OR al.[status] = @status)
              AND (@hasSearch = 0 OR al.[lead_name] LIKE @searchPattern OR al.[lead_phone] LIKE @searchPattern OR (al.[lead_email] IS NOT NULL AND al.[lead_email] LIKE @searchPattern))
            ORDER BY al.[created_at] DESC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

        command.Parameters.Add(new SqlParameter("@agentId", SqlDbType.Int) { Value = (object?)request.AgentId ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@status", SqlDbType.NVarChar, 50) { Value = (object?)request.Status ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@hasSearch", SqlDbType.Bit) { Value = hasSearch });
        command.Parameters.Add(new SqlParameter("@searchPattern", SqlDbType.NVarChar, 100) { Value = (object?)searchPattern ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
        command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

        int totalCount = 0;
        var items = new List<AgentLeadDto>(pageSize);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            if (totalCount == 0)
            {
                totalCount = reader.GetInt32(0);
            }
            items.Add(AgentLeadQueryMapping.ReadAgentLeadDto(reader, offset: 1));
        }

        if (totalCount == 0 && offset > 0)
        {
            await using var countCmd = connection.CreateCommand();
            countCmd.CommandText = @"
                SELECT COUNT(1)
                FROM [AgentLeads] al
                WHERE (@agentId IS NULL OR al.[agent_id] = @agentId)
                  AND (@status IS NULL OR al.[status] = @status)
                  AND (@hasSearch = 0 OR al.[lead_name] LIKE @searchPattern OR al.[lead_phone] LIKE @searchPattern OR (al.[lead_email] IS NOT NULL AND al.[lead_email] LIKE @searchPattern));";
            countCmd.Parameters.Add(new SqlParameter("@agentId", SqlDbType.Int) { Value = (object?)request.AgentId ?? DBNull.Value });
            countCmd.Parameters.Add(new SqlParameter("@status", SqlDbType.NVarChar, 50) { Value = (object?)request.Status ?? DBNull.Value });
            countCmd.Parameters.Add(new SqlParameter("@hasSearch", SqlDbType.Bit) { Value = hasSearch });
            countCmd.Parameters.Add(new SqlParameter("@searchPattern", SqlDbType.NVarChar, 100) { Value = (object?)searchPattern ?? DBNull.Value });
            var scalar = await countCmd.ExecuteScalarAsync(cancellationToken);
            totalCount = Convert.ToInt32(scalar);
        }

        return new PaginatedList<AgentLeadDto>(items, totalCount, pageNumber, pageSize);
    }
}
