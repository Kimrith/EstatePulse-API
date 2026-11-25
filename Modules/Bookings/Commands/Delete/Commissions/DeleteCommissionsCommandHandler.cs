using EstatePulse.Common.Database;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Commands.Delete;

public sealed class DeleteCommissionsCommandHandler : IRequestHandler<DeleteCommissionsCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteCommissionsCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<bool> Handle(DeleteCommissionsCommand request, CancellationToken cancellationToken)
    {
        var commission = await _context.Commissions
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (commission == null)
        {
            return false;
        }

        _context.Commissions.Remove(commission);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
