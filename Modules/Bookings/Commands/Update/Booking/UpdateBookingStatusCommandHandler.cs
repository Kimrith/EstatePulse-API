using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Booking;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Commands.Update;

public sealed class UpdateBookingStatusCommandHandler : IRequestHandler<UpdateBookingStatusCommand, BookingDto?>
{
    private readonly AppDbContext _context;

    public UpdateBookingStatusCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<BookingDto?> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            return null;
        }

        booking.Status = request.Status;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var unitCode = await _context.Units
            .Where(u => u.Id == booking.UnitId)
            .Select(u => u.UnitCode)
            .FirstOrDefaultAsync(cancellationToken);

        var customer = await _context.Users
            .Where(u => u.Id == booking.CustomerId)
            .Select(u => new { u.FullName, u.Email, u.PhoneNumber })
            .FirstOrDefaultAsync(cancellationToken);

        string? agentCode = null;
        if (booking.AgentId.HasValue)
        {
            agentCode = await _context.Agents
                .Where(a => a.Id == booking.AgentId.Value)
                .Select(a => a.AgentCode)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new BookingDto
        {
            Id = booking.Id,
            BookingReference = booking.BookingReference,
            UnitId = booking.UnitId,
            CustomerId = booking.CustomerId,
            AgentId = booking.AgentId,
            AgreedPrice = booking.AgreedPrice,
            DiscountAmount = booking.DiscountAmount,
            DepositRequired = booking.DepositRequired,
            Status = booking.Status,
            ReservationExpiresAt = booking.ReservationExpiresAt,
            Notes = booking.Notes,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt,
            UnitCode = unitCode,
            CustomerName = customer?.FullName,
            CustomerEmail = customer?.Email,
            CustomerPhone = customer?.PhoneNumber,
            AgentCode = agentCode
        };
    }
}
