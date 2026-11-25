using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Commissions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Commands.Update;

public sealed class UpdateCommissionStatusCommandHandler : IRequestHandler<UpdateCommissionStatusCommand, CommissionDto?>
{
    private readonly AppDbContext _context;

    public UpdateCommissionStatusCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<CommissionDto?> Handle(UpdateCommissionStatusCommand request, CancellationToken cancellationToken)
    {
        var commission = await _context.Commissions
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (commission == null)
        {
            return null;
        }

        if (request.ApprovedByUserId.HasValue)
        {
            var approverExists = await _context.Users
                .AnyAsync(u => u.Id == request.ApprovedByUserId.Value, cancellationToken);

            if (!approverExists)
            {
                throw new KeyNotFoundException($"Approver user with ID {request.ApprovedByUserId.Value} does not exist.");
            }

            commission.ApprovedByUserId = request.ApprovedByUserId.Value;
        }

        var normalizedStatus = request.Status.Trim();
        commission.Status = normalizedStatus;

        if (string.Equals(normalizedStatus, "Approved", StringComparison.OrdinalIgnoreCase) && !commission.ApprovedAt.HasValue)
        {
            commission.ApprovedAt = DateTime.UtcNow;
        }
        else if (string.Equals(normalizedStatus, "Paid", StringComparison.OrdinalIgnoreCase) && !commission.PaidAt.HasValue)
        {
            commission.PaidAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await UpdateCommissionCommandHandler.BuildCommissionDtoAsync(_context, commission, cancellationToken);
    }
}
