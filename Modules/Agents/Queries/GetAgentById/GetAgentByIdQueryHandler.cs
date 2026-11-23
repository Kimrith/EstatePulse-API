using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Agents.DTOs.Agent;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Agents.Queries;

public sealed class GetAgentByIdQueryHandler : IRequestHandler<GetAgentByIdQuery, AgentDto?>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public GetAgentByIdQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<AgentDto?> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"agent-id-{request.Id}";

        if (_cache.TryGet(cacheKey, out AgentDto? cachedAgent))
        {
            return new ValueTask<AgentDto?>(cachedAgent);
        }

        return new ValueTask<AgentDto?>(FetchAndCacheAsync());

        async Task<AgentDto?> FetchAndCacheAsync()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    [id], 
                    [user_id], 
                    [agent_code], 
                    [license_number], 
                    [commission_rate], 
                    [assigned_branch], 
                    [total_deals_closed], 
                    [rating], 
                    [bio], 
                    [is_verified], 
                    [created_at], 
                    [updated_at]
                FROM [Agents]
                WHERE [id] = @id;";

            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = request.Id });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var dto = GetPaginatedAgentsQueryHandler.ReadAgentDto(reader);
                _cache.Set(cacheKey, dto, CacheDuration, prefix: "agents");
                return dto;
            }

            return null;
        }
    }
}
