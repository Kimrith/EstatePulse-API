using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Projects.Commands.Delete;

public sealed class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ICacheService _cache;

    public DeleteProjectCommandHandler(AppDbContext context, IWebHostEnvironment env, ICacheService cache)
    {
        _context = context;
        _env = env;
        _cache = cache;
    }

    public async ValueTask<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(project.ThumbnailUrl))
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relativePath = project.ThumbnailUrl.TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);
            var filePath = Path.Combine(webRoot, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("projects");

        return true;
    }
}
