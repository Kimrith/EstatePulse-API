using System.Data;
using System.Data.Common;
using EstatePulse.Data;
using EstatePulse.DTOs;
using EstatePulse.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EstatePulse.Queries
{
    public static class UserQueries
    {
        /// <summary>
        /// Reads a UserDto directly from a DbDataReader with zero ORM allocation overhead.
        /// Expected columns:
        /// 0: id, 1: full_name, 2: email, 3: phone_number, 4: role,
        /// 5: national_id_card, 6: avatar_url, 7: is_active, 8: created_at, 9: updated_at
        /// </summary>
        private static UserDto ReadUserDto(DbDataReader reader)
        {
            var roleStr = reader.IsDBNull(4) ? null : reader.GetString(4);
            Enum.TryParse<UserRole>(roleStr, true, out var role);

            return new UserDto
            {
                Id = reader.GetInt32(0),
                FullName = reader.GetString(1),
                Email = reader.GetString(2),
                PhoneNumber = reader.GetString(3),
                Role = role,
                NationalIdCard = reader.IsDBNull(5) ? null : reader.GetString(5),
                AvatarUrl = reader.IsDBNull(6) ? null : reader.GetString(6),
                IsActive = reader.GetBoolean(7),
                CreatedAt = reader.GetDateTime(8),
                UpdatedAt = reader.IsDBNull(9) ? null : reader.GetDateTime(9)
            };
        }

        public static async Task<PaginatedList<UserDto>> GetUsersAsync(
            AppDbContext context,
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 12,
            IMemoryCache? cache = null,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 12;

            var termKey = string.IsNullOrWhiteSpace(searchTerm) ? "all" : searchTerm.Trim().ToLowerInvariant();
            var cacheKey = $"users-list-{termKey}-page-{pageNumber}-size-{pageSize}";

            if (cache != null && cache.TryGetValue(cacheKey, out PaginatedList<UserDto>? cached) && cached != null)
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

            bool hasSearch = !string.IsNullOrWhiteSpace(searchTerm);

            if (hasSearch)
            {
                var term = $"%{searchTerm!.Trim()}%";

                // Combined in ONE single round-trip: Total Count + Paginated Rows
                command.CommandText = @"
                    -- 1. Total Count
                    SELECT COUNT(1) 
                    FROM [Users] WITH (NOLOCK)
                    WHERE [full_name] LIKE @searchTerm OR [email] LIKE @searchTerm;

                    -- 2. Paginated Data
                    SELECT [id], [full_name], [email], [phone_number], [role], 
                           [national_id_card], [avatar_url], [is_active], [created_at], [updated_at]
                    FROM [Users] WITH (NOLOCK)
                    WHERE [full_name] LIKE @searchTerm OR [email] LIKE @searchTerm
                    ORDER BY [created_at] DESC
                    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

                command.Parameters.Add(new SqlParameter("@searchTerm", SqlDbType.NVarChar, 255) { Value = term });
                command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
                command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });
            }
            else
            {
                // Combined in ONE single round-trip: Total Count + Paginated Rows
                command.CommandText = @"
                    -- 1. Total Count
                    SELECT COUNT(1) 
                    FROM [Users] WITH (NOLOCK);

                    -- 2. Paginated Data
                    SELECT [id], [full_name], [email], [phone_number], [role], 
                           [national_id_card], [avatar_url], [is_active], [created_at], [updated_at]
                    FROM [Users] WITH (NOLOCK)
                    ORDER BY [created_at] DESC
                    OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

                command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
                command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });
            }

            int totalCount = 0;
            var items = new List<UserDto>(pageSize);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            // Read Result Set 1: Total Count
            if (await reader.ReadAsync(cancellationToken))
            {
                totalCount = reader.GetInt32(0);
            }

            // Read Result Set 2: Paginated Items
            if (await reader.NextResultAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(ReadUserDto(reader));
                }
            }

            var result = new PaginatedList<UserDto>(items, totalCount, pageNumber, pageSize);
            cache?.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public static async Task<UserDto?> GetUserByIdAsync(
            AppDbContext context,
            int id,
            IMemoryCache? cache = null,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"user-id-{id}";
            if (cache != null && cache.TryGetValue(cacheKey, out UserDto? cachedUser))
            {
                return cachedUser;
            }

            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT TOP(1) [id], [full_name], [email], [phone_number], [role], 
                       [national_id_card], [avatar_url], [is_active], [created_at], [updated_at]
                FROM [Users] WITH (NOLOCK)
                WHERE [id] = @id;";

            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var user = ReadUserDto(reader);
                cache?.Set(cacheKey, user, TimeSpan.FromMinutes(10));
                return user;
            }

            return null;
        }

        public static async Task<UserDto?> GetUserByNameAsync(
            AppDbContext context,
            string fullName,
            IMemoryCache? cache = null,
            CancellationToken cancellationToken = default)
        {
            var normalizedName = fullName.Trim().ToLowerInvariant();
            var cacheKey = $"user-name-{normalizedName}";
            if (cache != null && cache.TryGetValue(cacheKey, out UserDto? cachedUser))
            {
                return cachedUser;
            }

            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT TOP(1) [id], [full_name], [email], [phone_number], [role], 
                       [national_id_card], [avatar_url], [is_active], [created_at], [updated_at]
                FROM [Users] WITH (NOLOCK)
                WHERE LOWER([full_name]) = LOWER(@fullName);";

            command.Parameters.Add(new SqlParameter("@fullName", SqlDbType.NVarChar, 150) { Value = fullName.Trim() });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var user = ReadUserDto(reader);
                cache?.Set(cacheKey, user, TimeSpan.FromMinutes(10));
                return user;
            }

            return null;
        }

        public static async Task<UserDto?> GetUserByEmailAsync(
            AppDbContext context,
            string email,
            IMemoryCache? cache = null,
            CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var cacheKey = $"user-email-{normalizedEmail}";
            if (cache != null && cache.TryGetValue(cacheKey, out UserDto? cachedUser))
            {
                return cachedUser;
            }

            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT TOP(1) [id], [full_name], [email], [phone_number], [role], 
                       [national_id_card], [avatar_url], [is_active], [created_at], [updated_at]
                FROM [Users] WITH (NOLOCK)
                WHERE LOWER([email]) = LOWER(@email);";

            command.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar, 255) { Value = email.Trim() });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var user = ReadUserDto(reader);
                cache?.Set(cacheKey, user, TimeSpan.FromMinutes(10));
                return user;
            }

            return null;
        }
    }
}
