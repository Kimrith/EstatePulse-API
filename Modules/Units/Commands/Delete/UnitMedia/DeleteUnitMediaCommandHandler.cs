using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Commands.Delete;

public sealed class DeleteUnitMediaCommandHandler : IRequestHandler<DeleteUnitMediaCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public DeleteUnitMediaCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<bool> Handle(DeleteUnitMediaCommand request, CancellationToken cancellationToken)
    {
        var media = await _context.UnitMedia
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (media == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(media.MediaUrl) && media.MediaUrl.StartsWith("/uploads/units/"))
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relativePath = media.MediaUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var filePath = Path.Combine(webRoot, relativePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        _context.UnitMedia.Remove(media);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("unit-media");

        return true;
    }
}
