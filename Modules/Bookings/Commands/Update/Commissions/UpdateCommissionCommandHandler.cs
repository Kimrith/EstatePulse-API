using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Commissions;
using EstatePulse.Modules.Bookings.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Commands.Update;

public sealed class UpdateCommissionCommandHandler : IRequestHandler<UpdateCommissionCommand, CommissionDto?>
{
    private readonly AppDbContext _context;

    public UpdateCommissionCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<CommissionDto?> Handle(UpdateCommissionCommand request, CancellationToken cancellationToken)
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

        if (request.CommissionRate.HasValue)
        {
            commission.CommissionRate = request.CommissionRate.Value;
        }

        if (request.CommissionAmount.HasValue)
        {
            commission.CommissionAmount = request.CommissionAmount.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            commission.Status = request.Status.Trim();
        }

        if (request.ApprovedAt.HasValue)
        {
            commission.ApprovedAt = request.ApprovedAt.Value;
        }

        if (request.PaidAt.HasValue)
        {
            commission.PaidAt = request.PaidAt.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await BuildCommissionDtoAsync(_context, commission, cancellationToken);
    }

    internal static async Task<CommissionDto> BuildCommissionDtoAsync(
        AppDbContext context,
        Commission commission,
        CancellationToken cancellationToken)
    {
        var bookingReference = await context.Bookings
            .Where(b => b.Id == commission.BookingId)
            .Select(b => b.BookingReference)
            .FirstOrDefaultAsync(cancellationToken);

        var agentCode = await context.Agents
            .Where(a => a.Id == commission.AgentId)
            .Select(a => a.AgentCode)
            .FirstOrDefaultAsync(cancellationToken);

        string? approvedByUserName = null;
        if (commission.ApprovedByUserId.HasValue)
        {
            approvedByUserName = await context.Users
                .Where(u => u.Id == commission.ApprovedByUserId.Value)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new CommissionDto
        {
            Id = commission.Id,
            BookingId = commission.BookingId,
            AgentId = commission.AgentId,
            CommissionRate = commission.CommissionRate,
            CommissionAmount = commission.CommissionAmount,
            Status = commission.Status,
            ApprovedByUserId = commission.ApprovedByUserId,
            ApprovedAt = commission.ApprovedAt,
            PaidAt = commission.PaidAt,
            CreatedAt = commission.CreatedAt,
            BookingReference = bookingReference,
            AgentCode = agentCode,
            ApprovedByUserName = approvedByUserName
        };
    }
}
