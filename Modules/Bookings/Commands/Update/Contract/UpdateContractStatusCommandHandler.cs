using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Contract;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Commands.Update;

public sealed class UpdateContractStatusCommandHandler : IRequestHandler<UpdateContractStatusCommand, ContractDto?>
{
    private readonly AppDbContext _context;

    public UpdateContractStatusCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<ContractDto?> Handle(UpdateContractStatusCommand request, CancellationToken cancellationToken)
    {
        var contract = await _context.Contracts
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (contract == null)
        {
            return null;
        }

        var normalizedStatus = request.Status.Trim();
        contract.Status = normalizedStatus;

        if (string.Equals(normalizedStatus, "Signed", StringComparison.OrdinalIgnoreCase) && !contract.SignedAt.HasValue)
        {
            contract.SignedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await UpdateContractCommandHandler.BuildContractDtoAsync(_context, contract, cancellationToken);
    }
}
