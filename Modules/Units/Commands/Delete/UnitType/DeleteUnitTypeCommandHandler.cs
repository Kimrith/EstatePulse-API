using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Commands.Delete;

public sealed class DeleteUnitTypeCommandHandler : IRequestHandler<DeleteUnitTypeCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public DeleteUnitTypeCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<bool> Handle(DeleteUnitTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.UnitTypes
            .FirstOrDefaultAsync(ut => ut.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        var unitsCount = await _context.Units
            .CountAsync(u => u.UnitTypeId == entity.Id, cancellationToken);

        if (unitsCount > 0)
        {
            throw new InvalidOperationException($"Cannot delete Unit Type '{entity.Code}' because {unitsCount} units are assigned to it.");
        }

        if (!string.IsNullOrEmpty(entity.FloorPlanImageUrl) && entity.FloorPlanImageUrl.StartsWith("/uploads/unit-types/"))
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relativePath = entity.FloorPlanImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var filePath = Path.Combine(webRoot, relativePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        _context.UnitTypes.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("unit-types");

        return true;
    }
}
