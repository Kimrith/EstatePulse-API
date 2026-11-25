using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Commissions;
using EstatePulse.Modules.Bookings.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Commands.Create;

public sealed class CreateCommissionsCommandHandler : IRequestHandler<CreateCommissionsCommand, CommissionDto>
{
    private readonly AppDbContext _context;

    public CreateCommissionsCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<CommissionDto> Handle(CreateCommissionsCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking with ID {request.BookingId} does not exist.");
        }

        var agent = await _context.Agents
            .FirstOrDefaultAsync(a => a.Id == request.AgentId, cancellationToken);

        if (agent == null)
        {
            throw new KeyNotFoundException($"Agent with ID {request.AgentId} does not exist.");
        }

        decimal commissionAmount = request.CommissionAmount
            ?? Math.Round(booking.AgreedPrice * (request.CommissionRate / 100m), 2, MidpointRounding.AwayFromZero);

        var status = string.IsNullOrWhiteSpace(request.Status) ? "Pending" : request.Status.Trim();

        var commission = new Commission
        {
            BookingId = request.BookingId,
            AgentId = request.AgentId,
            CommissionRate = request.CommissionRate,
            CommissionAmount = commissionAmount,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        _context.Commissions.Add(commission);
        await _context.SaveChangesAsync(cancellationToken);

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
            BookingReference = booking.BookingReference,
            AgentCode = agent.AgentCode,
            ApprovedByUserName = null
        };
    }
}
