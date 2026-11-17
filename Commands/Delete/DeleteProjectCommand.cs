using EstatePulse.Data;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Commands.Delete;

public class DeleteProjectCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public DeleteProjectCommandHandler(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async ValueTask<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (project == null)
        {
            return false;
        }

        // 1. Delete associated thumbnail image from disk if it exists
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

        // 2. Remove project from database
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}