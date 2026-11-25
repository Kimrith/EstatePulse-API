using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Projects.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Projects.Queries;

public sealed class GetProjectPhaseBySlugQueryHandler : IRequestHandler<GetProjectPhaseBySlugQuery, ProjectPhaseDto?>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public GetProjectPhaseBySlugQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<ProjectPhaseDto?> Handle(GetProjectPhaseBySlugQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"phase-slug-{request.ProjectId}-{request.Slug.ToLowerInvariant()}";

        if (_cache.TryGet(cacheKey, out ProjectPhaseDto? cachedPhase))
        {
            return new ValueTask<ProjectPhaseDto?>(cachedPhase);
        }

        return new ValueTask<ProjectPhaseDto?>(FetchAndCacheAsync());

        async Task<ProjectPhaseDto?> FetchAndCacheAsync()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
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
                WHERE pp.[project_id] = @projectId AND LOWER(pp.[slug]) = LOWER(@slug);";

            command.Parameters.Add(new SqlParameter("@projectId", SqlDbType.Int) { Value = request.ProjectId });
            command.Parameters.Add(new SqlParameter("@slug", SqlDbType.NVarChar, 150) { Value = request.Slug.Trim() });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var dto = ProjectPhaseQueryMapping.ReadProjectPhaseDto(reader);
                _cache.Set(cacheKey, dto, CacheDuration, prefix: "phases");
                return dto;
            }

            return null;
        }
    }
}
