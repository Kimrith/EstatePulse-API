using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Projects.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Projects.Queries;

public sealed class GetProjectBySlugQueryHandler : IRequestHandler<GetProjectBySlugQuery, ProjectDto?>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public GetProjectBySlugQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<ProjectDto?> Handle(GetProjectBySlugQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"project-slug-{request.Slug.ToLowerInvariant()}";

        if (_cache.TryGet(cacheKey, out ProjectDto? cachedProject))
        {
            return new ValueTask<ProjectDto?>(cachedProject);
        }

        return new ValueTask<ProjectDto?>(FetchAndCacheAsync());

        async Task<ProjectDto?> FetchAndCacheAsync()
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
                    [name],
                    [slug],
                    [location_address],
                    [city],
                    [latitude],
                    [longitude],
                    [thumbnail_url],
                    [description],
                    [is_published],
                    [created_at]
                FROM [Projects]
                WHERE LOWER([slug]) = LOWER(@slug);";

            command.Parameters.Add(new SqlParameter("@slug", SqlDbType.NVarChar, 200) { Value = request.Slug.Trim() });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var project = ProjectQueryMapping.ReadProjectDto(reader);
                _cache.Set(cacheKey, project, CacheDuration, prefix: "projects");
                return project;
            }

            return null;
        }
    }
}
