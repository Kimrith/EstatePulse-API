using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Common.Results;
using EstatePulse.Modules.Agents.DTOs.Agent;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Agents.Queries;

public sealed class GetPaginatedAgentsQueryHandler : IRequestHandler<GetPaginatedAgentsQuery, PaginatedList<AgentDto>>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public GetPaginatedAgentsQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<PaginatedList<AgentDto>> Handle(GetPaginatedAgentsQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        int pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        var cacheKey = $"agents-page-{pageNumber}-size-{pageSize}";

        if (_cache.TryGet(cacheKey, out PaginatedList<AgentDto>? cachedList) && cachedList != null)
        {
            return new ValueTask<PaginatedList<AgentDto>>(cachedList);
        }

        return new ValueTask<PaginatedList<AgentDto>>(FetchAndCacheAsync());

        async Task<PaginatedList<AgentDto>> FetchAndCacheAsync()
        {
            int offset = (pageNumber - 1) * pageSize;
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT 
                    COUNT(*) OVER() AS [TotalCount],
                    a.[id],
                    a.[user_id],
                    a.[agent_code],
                    a.[license_number],
                    a.[commission_rate],
                    a.[assigned_branch],
                    a.[total_deals_closed],
                    a.[rating],
                    a.[bio],
                    a.[is_verified],
                    a.[created_at],
                    a.[updated_at]
                FROM [Agents] a
                ORDER BY a.[created_at] DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
            command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

            int totalCount = 0;
            var items = new List<AgentDto>(pageSize);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                if (totalCount == 0)
                {
                    totalCount = reader.GetInt32(0);
                }

                items.Add(AgentQueryMapping.ReadAgentDto(reader, 1));
            }

            if (totalCount == 0 && offset > 0)
            {
                await using var countCmd = connection.CreateCommand();
                countCmd.CommandText = "SELECT COUNT(1) FROM [Agents];";
                var scalar = await countCmd.ExecuteScalarAsync(cancellationToken);
                totalCount = Convert.ToInt32(scalar);
            }

            var result = new PaginatedList<AgentDto>(items, totalCount, pageNumber, pageSize);
            _cache.Set(cacheKey, result, CacheDuration, prefix: "agents");
            return result;
        }
    }
}
