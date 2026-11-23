using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Projects.Commands.Delete;

public sealed class DeleteProjectPhaseCommandHandler : IRequestHandler<DeleteProjectPhaseCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public DeleteProjectPhaseCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<bool> Handle(DeleteProjectPhaseCommand request, CancellationToken cancellationToken)
    {
        var phase = await _context.ProjectPhases
            .FirstOrDefaultAsync(pp => pp.Id == request.Id, cancellationToken);

        if (phase == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(phase.MasterPlanSvgUrl))
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relativePath = phase.MasterPlanSvgUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var filePath = Path.Combine(webRoot, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        _context.ProjectPhases.Remove(phase);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("phases");

        return true;
    }
}
