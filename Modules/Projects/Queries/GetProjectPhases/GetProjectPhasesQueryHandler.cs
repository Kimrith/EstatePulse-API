using System.Data;
using System.Data.Common;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Common.Results;
using EstatePulse.Modules.Projects.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Projects.Queries;

public sealed class GetProjectPhasesQueryHandler : IRequestHandler<GetProjectPhasesQuery, PaginatedList<ProjectPhaseDto>>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public GetProjectPhasesQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<PaginatedList<ProjectPhaseDto>> Handle(GetProjectPhasesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        var projectKey = request.ProjectId?.ToString() ?? "all";
        var cacheKey = $"phases-proj-{projectKey}-page-{pageNumber}-size-{pageSize}";

        if (_cache.TryGet(cacheKey, out PaginatedList<ProjectPhaseDto>? cachedList) && cachedList != null)
        {
            return new ValueTask<PaginatedList<ProjectPhaseDto>>(cachedList);
        }

        return new ValueTask<PaginatedList<ProjectPhaseDto>>(FetchAndCacheAsync());

        async Task<PaginatedList<ProjectPhaseDto>> FetchAndCacheAsync()
        {
            var offset = (pageNumber - 1) * pageSize;
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT 
                    COUNT(*) OVER() AS [TotalCount],
                    pp.[id],
                    pp.[project_id],
                    pp.[phase_name],
                    pp.[slug],
                    pp.[master_plan_svg_url],
                    pp.[launch_date],
                    pp.[expected_completion_date],
                    pp.[status],
                    pp.[created_at],
                    p.[name] AS [project_name]
                FROM [ProjectPhases] pp
                INNER JOIN [Projects] p ON p.[id] = pp.[project_id]
                WHERE (@projectId IS NULL OR pp.[project_id] = @projectId)
                ORDER BY pp.[created_at] DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            command.Parameters.Add(new SqlParameter("@projectId", SqlDbType.Int) { Value = (object?)request.ProjectId ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
            command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

            int totalCount = 0;
            var items = new List<ProjectPhaseDto>(pageSize);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                if (totalCount == 0)
                {
                    totalCount = reader.GetInt32(0);
                }

                items.Add(ProjectPhaseQueryMapping.ReadProjectPhaseDto(reader, offset: 1));
            }

            if (totalCount == 0 && offset > 0)
            {
                await using var countCmd = connection.CreateCommand();
                countCmd.CommandText = @"
                    SELECT COUNT(1)
                    FROM [ProjectPhases] pp
                    WHERE (@projectId IS NULL OR pp.[project_id] = @projectId);";
                countCmd.Parameters.Add(new SqlParameter("@projectId", SqlDbType.Int) { Value = (object?)request.ProjectId ?? DBNull.Value });
                var scalar = await countCmd.ExecuteScalarAsync(cancellationToken);
                totalCount = Convert.ToInt32(scalar);
            }

            var result = new PaginatedList<ProjectPhaseDto>(items, totalCount, pageNumber, pageSize);
            _cache.Set(cacheKey, result, CacheDuration, prefix: "phases");
            return result;
        }
    }
}
