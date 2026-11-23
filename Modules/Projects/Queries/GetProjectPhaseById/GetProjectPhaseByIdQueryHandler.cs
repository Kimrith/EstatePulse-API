using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Projects.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Projects.Queries;

public sealed class GetProjectPhaseByIdQueryHandler : IRequestHandler<GetProjectPhaseByIdQuery, ProjectPhaseDto?>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public GetProjectPhaseByIdQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<ProjectPhaseDto?> Handle(GetProjectPhaseByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"phase-id-{request.Id}";

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
                WHERE pp.[id] = @id;";

            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = request.Id });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var dto = GetProjectPhasesQueryHandler.ReadProjectPhaseDto(reader);
                _cache.Set(cacheKey, dto, CacheDuration, prefix: "phases");
                return dto;
            }

            return null;
        }
    }
}
