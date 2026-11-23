using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Common.Results;
using EstatePulse.Modules.Users.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Users.Queries;

public sealed class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, PaginatedList<UserFollowsDto>>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    public GetFollowersQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<PaginatedList<UserFollowsDto>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 12 : request.PageSize;
        var cacheKey = $"user-followers-{request.UserId}-page-{pageNumber}-size-{pageSize}";

        if (_cache.TryGet(cacheKey, out PaginatedList<UserFollowsDto>? cached) && cached != null)
        {
            return new ValueTask<PaginatedList<UserFollowsDto>>(cached);
        }

        return new ValueTask<PaginatedList<UserFollowsDto>>(FetchAndCacheAsync());

        async Task<PaginatedList<UserFollowsDto>> FetchAndCacheAsync()
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
                    uf.[id],
                    uf.[following_user_id],
                    uf.[followed_user_id],
                    uf.[created_at],
                    u.[id] AS [follower_id],
                    u.[full_name],
                    u.[email],
                    u.[avatar_url]
                FROM [UserFollows] uf
                INNER JOIN [Users] u ON u.[id] = uf.[following_user_id]
                WHERE uf.[followed_user_id] = @userId
                ORDER BY uf.[created_at] DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            command.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = request.UserId });
            command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
            command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

            int totalCount = 0;
            var items = new List<UserFollowsDto>(pageSize);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                if (totalCount == 0)
                {
                    totalCount = reader.GetInt32(0);
                }

                items.Add(new UserFollowsDto
                {
                    Id = reader.GetInt32(1),
                    FollowingUserId = reader.GetInt32(2),
                    FollowedUserId = reader.GetInt32(3),
                    CreatedAt = reader.GetDateTime(4),
                    FollowingUser = new UserSummaryDto
                    {
                        Id = reader.GetInt32(5),
                        FullName = reader.GetString(6),
                        Email = reader.GetString(7),
                        AvatarUrl = reader.IsDBNull(8) ? null : reader.GetString(8)
                    }
                });
            }

            if (totalCount == 0 && offset > 0)
            {
                await using var countCmd = connection.CreateCommand();
                countCmd.CommandText = "SELECT COUNT(1) FROM [UserFollows] WHERE [followed_user_id] = @userId;";
                countCmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = request.UserId });
                var scalar = await countCmd.ExecuteScalarAsync(cancellationToken);
                totalCount = Convert.ToInt32(scalar);
            }

            var result = new PaginatedList<UserFollowsDto>(items, totalCount, pageNumber, pageSize);
            _cache.Set(cacheKey, result, CacheDuration, prefix: "user-followers");
            return result;
        }
    }
}
