using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using Mediator;

namespace EstatePulse.Modules.Agents.Commands.Delete;

public sealed class DeleteAgentCommandHandler : IRequestHandler<DeleteAgentCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;

    public DeleteAgentCommandHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async ValueTask<bool> Handle(DeleteAgentCommand request, CancellationToken cancellationToken)
    {
        var agent = await _context.Agents.FindAsync(new object[] { request.Id }, cancellationToken);

        if (agent == null)
        {
            return false;
        }

        _context.Agents.Remove(agent);
        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("agents");

        return true;
    }
}
