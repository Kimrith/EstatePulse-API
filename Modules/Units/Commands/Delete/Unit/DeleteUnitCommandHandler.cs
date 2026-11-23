using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Commands.Delete;

public sealed class DeleteUnitCommandHandler : IRequestHandler<DeleteUnitCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public DeleteUnitCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<bool> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = await _context.Units
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (unit == null)
        {
            return false;
        }

        var bookingsCount = await _context.Bookings
            .CountAsync(b => b.UnitId == unit.Id, cancellationToken);

        if (bookingsCount > 0)
        {
            throw new InvalidOperationException($"Cannot delete unit '{unit.UnitCode}' because it has {bookingsCount} associated bookings.");
        }

        var mediaList = await _context.UnitMedia
            .Where(m => m.UnitId == unit.Id)
            .ToListAsync(cancellationToken);

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        // Delete associated unit media files from disk
        foreach (var media in mediaList)
        {
            if (!string.IsNullOrEmpty(media.MediaUrl) && media.MediaUrl.StartsWith("/uploads/units/"))
            {
                var relativePath = media.MediaUrl.TrimStart('/')
                    .Replace('/', Path.DirectorySeparatorChar);
                var filePath = Path.Combine(webRoot, relativePath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        _context.UnitMedia.RemoveRange(mediaList);
        _context.Units.Remove(unit);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("unit-types");

        return true;
    }
}
