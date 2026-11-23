using System.Data;
using EstatePulse.Common.Database;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Users.Queries;

public sealed class IsFollowingQueryHandler : IRequestHandler<IsFollowingQuery, bool>
{
    private readonly AppDbContext _context;

    public IsFollowingQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<bool> Handle(IsFollowingQuery request, CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT TOP(1) 1
            FROM [UserFollows] WITH (NOLOCK)
            WHERE [following_user_id] = @followerId AND [followed_user_id] = @followedId;";

        command.Parameters.Add(new SqlParameter("@followerId", SqlDbType.Int) { Value = request.FollowerId });
        command.Parameters.Add(new SqlParameter("@followedId", SqlDbType.Int) { Value = request.FollowedId });

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result != null && result != DBNull.Value;
    }
}
