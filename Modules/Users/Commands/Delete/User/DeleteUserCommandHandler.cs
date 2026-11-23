using System.IO;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Users.Commands.Delete;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;

    public DeleteUserCommandHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async ValueTask<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(user.AvatarUrl) && user.AvatarUrl.StartsWith("/uploads/avatars/"))
        {
            var relativePath = user.AvatarUrl.TrimStart('/');
            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("users");

        return true;
    }
}
