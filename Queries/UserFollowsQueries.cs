using System.Data;
using System.Data.Common;
using EstatePulse.Data;
using EstatePulse.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EstatePulse.Queries
{
    public static class UserFollowsQueries
    {
        public static async Task<PaginatedList<UserFollowsDto>> GetFollowersAsync(
            AppDbContext context,
            int userId,
            int pageNumber = 1,
            int pageSize = 12,
            IMemoryCache? cache = null,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 12;

            var cacheKey = $"user-followers-{userId}-page-{pageNumber}-size-{pageSize}";
            if (cache != null && cache.TryGetValue(cacheKey, out PaginatedList<UserFollowsDto>? cached) && cached != null)
            {
                return cached;
            }

            int offset = (pageNumber - 1) * pageSize;

            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();

            // Combined in ONE database round-trip: Total Count + Joined Paginated Data
            command.CommandText = @"
                -- 1. Total Count
                SELECT COUNT(1) 
                FROM [UserFollows] WITH (NOLOCK) 
                WHERE [followed_user_id] = @userId;

                -- 2. Paginated Followers with User Summary Join
                SELECT 
                    uf.[id], 
                    uf.[following_user_id], 
                    uf.[followed_user_id], 
                    uf.[created_at],
                    u.[id] AS [u_id], 
                    u.[full_name], 
                    u.[email], 
                    u.[avatar_url]
                FROM [UserFollows] uf WITH (NOLOCK)
                INNER JOIN [Users] u WITH (NOLOCK) ON uf.[following_user_id] = u.[id]
                WHERE uf.[followed_user_id] = @userId
                ORDER BY uf.[created_at] DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            command.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
            command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
            command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

            int totalCount = 0;
            var items = new List<UserFollowsDto>(pageSize);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            // Read Result Set 1: Count
            if (await reader.ReadAsync(cancellationToken))
            {
                totalCount = reader.GetInt32(0);
            }

            // Read Result Set 2: Data
            if (await reader.NextResultAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new UserFollowsDto
                    {
                        Id = reader.GetInt32(0),
                        FollowingUserId = reader.GetInt32(1),
                        FollowedUserId = reader.GetInt32(2),
                        CreatedAt = reader.GetDateTime(3),
                        FollowingUser = new UserSummaryDto
                        {
                            Id = reader.GetInt32(4),
                            FullName = reader.GetString(5),
                            Email = reader.GetString(6),
                            AvatarUrl = reader.IsDBNull(7) ? null : reader.GetString(7)
                        }
                    });
                }
            }

            var result = new PaginatedList<UserFollowsDto>(items, totalCount, pageNumber, pageSize);
            cache?.Set(cacheKey, result, TimeSpan.FromMinutes(3));
            return result;
        }

        public static async Task<PaginatedList<UserFollowsDto>> GetFollowingAsync(
            AppDbContext context,
            int userId,
            int pageNumber = 1,
            int pageSize = 12,
            IMemoryCache? cache = null,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 12;

            var cacheKey = $"user-following-{userId}-page-{pageNumber}-size-{pageSize}";
            if (cache != null && cache.TryGetValue(cacheKey, out PaginatedList<UserFollowsDto>? cached) && cached != null)
            {
                return cached;
            }

            int offset = (pageNumber - 1) * pageSize;

            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();

            // Combined in ONE database round-trip: Total Count + Joined Paginated Data
            command.CommandText = @"
                -- 1. Total Count
                SELECT COUNT(1) 
                FROM [UserFollows] WITH (NOLOCK) 
                WHERE [following_user_id] = @userId;

                -- 2. Paginated Followings with User Summary Join
                SELECT 
                    uf.[id], 
                    uf.[following_user_id], 
                    uf.[followed_user_id], 
                    uf.[created_at],
                    u.[id] AS [u_id], 
                    u.[full_name], 
                    u.[email], 
                    u.[avatar_url]
                FROM [UserFollows] uf WITH (NOLOCK)
                INNER JOIN [Users] u WITH (NOLOCK) ON uf.[followed_user_id] = u.[id]
                WHERE uf.[following_user_id] = @userId
                ORDER BY uf.[created_at] DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            command.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
            command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
            command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

            int totalCount = 0;
            var items = new List<UserFollowsDto>(pageSize);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            // Read Result Set 1: Count
            if (await reader.ReadAsync(cancellationToken))
            {
                totalCount = reader.GetInt32(0);
            }

            // Read Result Set 2: Data
            if (await reader.NextResultAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new UserFollowsDto
                    {
                        Id = reader.GetInt32(0),
                        FollowingUserId = reader.GetInt32(1),
                        FollowedUserId = reader.GetInt32(2),
                        CreatedAt = reader.GetDateTime(3),
                        FollowedUser = new UserSummaryDto
                        {
                            Id = reader.GetInt32(4),
                            FullName = reader.GetString(5),
                            Email = reader.GetString(6),
                            AvatarUrl = reader.IsDBNull(7) ? null : reader.GetString(7)
                        }
                    });
                }
            }

            var result = new PaginatedList<UserFollowsDto>(items, totalCount, pageNumber, pageSize);
            cache?.Set(cacheKey, result, TimeSpan.FromMinutes(3));
            return result;
        }

        public static async Task<bool> IsFollowingAsync(
            AppDbContext context,
            int followingUserId,
            int followedUserId,
            IMemoryCache? cache = null,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"user-is-following-{followingUserId}-{followedUserId}";
            if (cache != null && cache.TryGetValue(cacheKey, out bool cached))
            {
                return cached;
            }

            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 
                    FROM [UserFollows] WITH (NOLOCK)
                    WHERE [following_user_id] = @followingUserId AND [followed_user_id] = @followedUserId
                ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END;";

            command.Parameters.Add(new SqlParameter("@followingUserId", SqlDbType.Int) { Value = followingUserId });
            command.Parameters.Add(new SqlParameter("@followedUserId", SqlDbType.Int) { Value = followedUserId });

            var result = await command.ExecuteScalarAsync(cancellationToken);
            var isFollowing = result is bool b && b;
            cache?.Set(cacheKey, isFollowing, TimeSpan.FromMinutes(2));
            return isFollowing;
        }

        public static async Task<int> GetFollowersCountAsync(
            AppDbContext context,
            int userId,
            IMemoryCache? cache = null,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"user-followers-count-{userId}";
            if (cache != null && cache.TryGetValue(cacheKey, out int cachedCount))
            {
                return cachedCount;
            }

            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM [UserFollows] WITH (NOLOCK) WHERE [followed_user_id] = @userId;";
            command.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });

            var result = await command.ExecuteScalarAsync(cancellationToken);
            var count = result is int c ? c : Convert.ToInt32(result);
            cache?.Set(cacheKey, count, TimeSpan.FromMinutes(2));
            return count;
        }

        public static async Task<int> GetFollowingCountAsync(
            AppDbContext context,
            int userId,
            IMemoryCache? cache = null,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"user-following-count-{userId}";
            if (cache != null && cache.TryGetValue(cacheKey, out int cachedCount))
            {
                return cachedCount;
            }

            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM [UserFollows] WITH (NOLOCK) WHERE [following_user_id] = @userId;";
            command.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });

            var result = await command.ExecuteScalarAsync(cancellationToken);
            var count = result is int c ? c : Convert.ToInt32(result);
            cache?.Set(cacheKey, count, TimeSpan.FromMinutes(2));
            return count;
        }
    }
}
