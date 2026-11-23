using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Users.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Users.Queries;

public sealed class GetUserFollowCountsQueryHandler : IRequestHandler<GetUserFollowCountsQuery, UserFollowCountsDto>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public GetUserFollowCountsQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<UserFollowCountsDto> Handle(GetUserFollowCountsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"user-counts-{request.UserId}";

        if (_cache.TryGet(cacheKey, out UserFollowCountsDto? cached) && cached != null)
        {
            return new ValueTask<UserFollowCountsDto>(cached);
        }

        return new ValueTask<UserFollowCountsDto>(FetchAndCacheAsync());

        async Task<UserFollowCountsDto> FetchAndCacheAsync()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT 
                    (SELECT COUNT(1) FROM [UserFollows] WITH (NOLOCK) WHERE [followed_user_id] = @userId) AS [FollowersCount],
                    (SELECT COUNT(1) FROM [UserFollows] WITH (NOLOCK) WHERE [following_user_id] = @userId) AS [FollowingCount];";

            command.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = request.UserId });

            int followers = 0;
            int following = 0;

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                followers = reader.GetInt32(0);
                following = reader.GetInt32(1);
            }

            var counts = new UserFollowCountsDto
            {
                UserId = request.UserId,
                FollowersCount = followers,
                FollowingCount = following
            };

            _cache.Set(cacheKey, counts, CacheDuration, prefix: "user-counts");
            return counts;
        }
    }
}
