using EstatePulse.Data;
using EstatePulse.DTOs;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EstatePulse.Queries;

// 1. Get Paginated Projects Query & Handler
public class GetAllProjectsQuery : IRequest<PaginatedList<ProjectDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, PaginatedList<ProjectDto>>
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public GetAllProjectsQueryHandler(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<PaginatedList<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        var cacheKey = $"projects-page-{pageNumber}-size-{pageSize}";

        // Fast path: synchronous return from cache without async state machine overhead
        if (_cache.TryGetValue(cacheKey, out PaginatedList<ProjectDto>? cachedList) && cachedList != null)
        {
            return new ValueTask<PaginatedList<ProjectDto>>(cachedList);
        }

        return new ValueTask<PaginatedList<ProjectDto>>(FetchAndCacheAsync());

        async Task<PaginatedList<ProjectDto>> FetchAndCacheAsync()
        {
            var offset = (pageNumber - 1) * pageSize;

            // 1. Get total record count
            var countSql = """
                SELECT COUNT(*) AS Value
                FROM Projects
                """;

            var totalCount = await _context.Database
                .SqlQueryRaw<int>(countSql)
                .SingleAsync(cancellationToken);

            // 2. Get paginated items (SQL Server syntax)
            var dataSql = """
                SELECT 
                    id AS Id,
                    name AS Name,
                    slug AS Slug,
                    location_address AS LocationAddress,
                    city AS City,
                    latitude AS Latitude,
                    longitude AS Longitude,
                    thumbnail_url AS ThumbnailUrl,
                    description AS Description,
                    is_published AS IsPublished,
                    created_at AS CreatedAt
                FROM Projects
                ORDER BY created_at DESC
                OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY
                """;

            var items = totalCount > 0
                ? await _context.Database
                    .SqlQueryRaw<ProjectDto>(dataSql, offset, pageSize)
                    .ToListAsync(cancellationToken)
                : new List<ProjectDto>();

            var result = new PaginatedList<ProjectDto>(items, totalCount, pageNumber, pageSize);

            _cache.Set(cacheKey, result, CacheDuration);

            return result;
        }
    }
}

// 2. Get Project By ID Query & Handler
public class GetProjectByIdQuery : IRequest<ProjectDto?>
{
    public int Id { get; set; }
}

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public GetProjectByIdQueryHandler(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"project-id-{request.Id}";

        // Fast path: synchronous return on cache hit
        if (_cache.TryGetValue(cacheKey, out ProjectDto? cachedProject))
        {
            return new ValueTask<ProjectDto?>(cachedProject);
        }

        return new ValueTask<ProjectDto?>(FetchAndCacheAsync());

        async Task<ProjectDto?> FetchAndCacheAsync()
        {
            var sql = """
                SELECT 
                    id AS Id,
                    name AS Name,
                    slug AS Slug,
                    location_address AS LocationAddress,
                    city AS City,
                    latitude AS Latitude,
                    longitude AS Longitude,
                    thumbnail_url AS ThumbnailUrl,
                    description AS Description,
                    is_published AS IsPublished,
                    created_at AS CreatedAt
                FROM Projects
                WHERE id = {0}
                """;

            var project = await _context.Database
                .SqlQueryRaw<ProjectDto>(sql, request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (project != null)
            {
                _cache.Set(cacheKey, project, CacheDuration);
            }

            return project;
        }
    }
}

// 3. Get Project By Slug Query & Handler
public class GetProjectBySlugQuery : IRequest<ProjectDto?>
{
    public string Slug { get; set; } = string.Empty;
}

public class GetProjectBySlugQueryHandler : IRequestHandler<GetProjectBySlugQuery, ProjectDto?>
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public GetProjectBySlugQueryHandler(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<ProjectDto?> Handle(GetProjectBySlugQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"project-slug-{request.Slug.ToLowerInvariant()}";

        // Fast path: synchronous return on cache hit
        if (_cache.TryGetValue(cacheKey, out ProjectDto? cachedProject))
        {
            return new ValueTask<ProjectDto?>(cachedProject);
        }

        return new ValueTask<ProjectDto?>(FetchAndCacheAsync());

        async Task<ProjectDto?> FetchAndCacheAsync()
        {
            var sql = """
                SELECT 
                    id AS Id,
                    name AS Name,
                    slug AS Slug,
                    location_address AS LocationAddress,
                    city AS City,
                    latitude AS Latitude,
                    longitude AS Longitude,
                    thumbnail_url AS ThumbnailUrl,
                    description AS Description,
                    is_published AS IsPublished,
                    created_at AS CreatedAt
                FROM Projects
                WHERE slug = {0}
                """;

            var project = await _context.Database
                .SqlQueryRaw<ProjectDto>(sql, request.Slug)
                .FirstOrDefaultAsync(cancellationToken);

            if (project != null)
            {
                _cache.Set(cacheKey, project, CacheDuration);
            }

            return project;
        }
    }
}