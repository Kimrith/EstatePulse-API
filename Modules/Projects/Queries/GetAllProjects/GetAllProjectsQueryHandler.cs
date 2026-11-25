using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Common.Results;
using EstatePulse.Modules.Projects.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Projects.Queries;

public sealed class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, PaginatedList<ProjectDto>>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public GetAllProjectsQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<PaginatedList<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        var cacheKey = $"projects-page-{pageNumber}-size-{pageSize}";

        if (_cache.TryGet(cacheKey, out PaginatedList<ProjectDto>? cachedList) && cachedList != null)
        {
            return new ValueTask<PaginatedList<ProjectDto>>(cachedList);
        }

        return new ValueTask<PaginatedList<ProjectDto>>(FetchAndCacheAsync());

        async Task<PaginatedList<ProjectDto>> FetchAndCacheAsync()
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
                    p.[id],
                    p.[name],
                    p.[slug],
                    p.[location_address],
                    p.[city],
                    p.[latitude],
                    p.[longitude],
                    p.[thumbnail_url],
                    p.[description],
                    p.[is_published],
                    p.[created_at]
                FROM [Projects] p
                ORDER BY p.[created_at] DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
            command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

            int totalCount = 0;
            var items = new List<ProjectDto>(pageSize);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                if (totalCount == 0)
                {
                    totalCount = reader.GetInt32(0);
                }

                items.Add(ProjectQueryMapping.ReadProjectDto(reader, 1));
            }

            if (totalCount == 0 && offset > 0)
            {
                await using var countCmd = connection.CreateCommand();
                countCmd.CommandText = "SELECT COUNT(1) FROM [Projects];";
                var scalar = await countCmd.ExecuteScalarAsync(cancellationToken);
                totalCount = Convert.ToInt32(scalar);
            }

            var result = new PaginatedList<ProjectDto>(items, totalCount, pageNumber, pageSize);
            _cache.Set(cacheKey, result, CacheDuration, prefix: "projects");
            return result;
        }
    }
}
