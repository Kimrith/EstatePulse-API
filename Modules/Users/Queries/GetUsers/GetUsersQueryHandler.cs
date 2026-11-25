using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Common.Results;
using EstatePulse.Modules.Users.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Users.Queries;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedList<UserDto>>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public GetUsersQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<PaginatedList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 12 : request.PageSize;
        var termKey = string.IsNullOrWhiteSpace(request.SearchTerm) ? "all" : request.SearchTerm.Trim().ToLowerInvariant();
        var cacheKey = $"users-list-{termKey}-page-{pageNumber}-size-{pageSize}";

        if (_cache.TryGet(cacheKey, out PaginatedList<UserDto>? cached) && cached != null)
        {
            return new ValueTask<PaginatedList<UserDto>>(cached);
        }

        return new ValueTask<PaginatedList<UserDto>>(FetchAndCacheAsync());

        async Task<PaginatedList<UserDto>> FetchAndCacheAsync()
        {
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
                    [id], [full_name], [email], [phone_number], [role], 
                    [national_id_card], [avatar_url], [is_active], [created_at], [updated_at]
                FROM [Users] WITH (NOLOCK)
                WHERE (@hasSearch = 0 OR [full_name] LIKE @searchPattern OR [email] LIKE @searchPattern)
                ORDER BY [created_at] DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            command.Parameters.Add(new SqlParameter("@hasSearch", SqlDbType.Bit) { Value = hasSearch });
            command.Parameters.Add(new SqlParameter("@searchPattern", SqlDbType.NVarChar, 255) { Value = (object?)searchPattern ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
            command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

            int totalCount = 0;
            var items = new List<UserDto>(pageSize);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                if (totalCount == 0)
                {
                    totalCount = reader.GetInt32(0);
                }

                items.Add(UserQueryMapping.ReadUserDto(reader, offset: 1));
            }

            if (totalCount == 0 && offset > 0)
            {
                await using var countCmd = connection.CreateCommand();
                countCmd.CommandText = @"
                    SELECT COUNT(1) 
                    FROM [Users] WITH (NOLOCK)
                    WHERE (@hasSearch = 0 OR [full_name] LIKE @searchPattern OR [email] LIKE @searchPattern);";
                countCmd.Parameters.Add(new SqlParameter("@hasSearch", SqlDbType.Bit) { Value = hasSearch });
                countCmd.Parameters.Add(new SqlParameter("@searchPattern", SqlDbType.NVarChar, 255) { Value = (object?)searchPattern ?? DBNull.Value });
                var scalar = await countCmd.ExecuteScalarAsync(cancellationToken);
                totalCount = Convert.ToInt32(scalar);
            }

            var result = new PaginatedList<UserDto>(items, totalCount, pageNumber, pageSize);
            _cache.Set(cacheKey, result, CacheDuration, prefix: "users");
            return result;
        }
    }
}
