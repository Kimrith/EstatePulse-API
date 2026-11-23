using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Users.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Users.Queries;

public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public GetUserByIdQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"user-id-{request.Id}";

        if (_cache.TryGet(cacheKey, out UserDto? cachedUser))
        {
            return new ValueTask<UserDto?>(cachedUser);
        }

        return new ValueTask<UserDto?>(FetchAndCacheAsync());

        async Task<UserDto?> FetchAndCacheAsync()
        {
            var connection = _context.Database.GetDbConnection();
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

            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = request.Id });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var user = UserQueryMapping.ReadUserDto(reader);
                _cache.Set(cacheKey, user, CacheDuration, prefix: "users");
                return user;
            }

            return null;
        }
    }
}
