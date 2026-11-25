using EstatePulse.Common.Database;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Commands.Delete;

public sealed class DeleteContractCommandHandler : IRequestHandler<DeleteContractCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteContractCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<bool> Handle(DeleteContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _context.Contracts
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (contract == null)
        {
            return false;
        }

        _context.Contracts.Remove(contract);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
