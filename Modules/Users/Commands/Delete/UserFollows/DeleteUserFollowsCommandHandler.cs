using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Users.Commands.Delete;

public sealed class DeleteUserFollowsCommandHandler : IRequestHandler<DeleteUserFollowsCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;

    public DeleteUserFollowsCommandHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async ValueTask<bool> Handle(DeleteUserFollowsCommand request, CancellationToken cancellationToken)
    {
        int rowsAffected = await _context.UserFollows
            .Where(uf => uf.Id == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsAffected > 0)
        {
            _cache.RemoveByPrefix("user-followers");
            _cache.RemoveByPrefix("user-following");
            _cache.RemoveByPrefix("user-counts");
        }

        return rowsAffected > 0;
    }
}
