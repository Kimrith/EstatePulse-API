using System.Data;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Agents.DTOs.AgentLead;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Agents.Queries;

public sealed class GetAgentLeadByIdQueryHandler : IRequestHandler<GetAgentLeadByIdQuery, AgentLeadDto?>
{
    private readonly AppDbContext _context;

    public GetAgentLeadByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<AgentLeadDto?> Handle(GetAgentLeadByIdQuery request, CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
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
            WHERE al.[id] = @id;";

        command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = request.Id });

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return GetPaginatedAgentLeadsQueryHandler.ReadAgentLeadDto(reader);
        }

        return null;
    }
}
