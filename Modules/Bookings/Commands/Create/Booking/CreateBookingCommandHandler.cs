using EstatePulse.Common.Database;
using EstatePulse.Modules.Bookings.DTOs.Booking;
using EstatePulse.Modules.Bookings.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Commands.Create;

public sealed class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly AppDbContext _context;

    public CreateBookingCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify Unit exists
        var unit = await _context.Units
            .FirstOrDefaultAsync(u => u.Id == request.UnitId, cancellationToken);

        if (unit == null)
        {
            throw new KeyNotFoundException($"Unit with ID {request.UnitId} does not exist.");
        }

        // 2. Verify Customer exists
        var customer = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.CustomerId, cancellationToken);

        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID {request.CustomerId} does not exist.");
        }

        // 3. Verify Agent exists if specified
        string? agentCode = null;
        if (request.AgentId.HasValue)
        {
            var agent = await _context.Agents
                .FirstOrDefaultAsync(a => a.Id == request.AgentId.Value, cancellationToken);

            if (agent == null)
            {
                throw new KeyNotFoundException($"Agent with ID {request.AgentId.Value} does not exist.");
            }

            agentCode = agent.AgentCode;
        }

        // 4. Generate unique reference code
        var bookingReference = $"BK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

        var booking = new Booking
        {
            BookingReference = bookingReference,
            UnitId = request.UnitId,
            CustomerId = request.CustomerId,
            AgentId = request.AgentId,
            AgreedPrice = request.AgreedPrice,
            DiscountAmount = request.DiscountAmount,
            DepositRequired = request.DepositRequired,
            Status = string.IsNullOrWhiteSpace(request.Status) ? Booking.Statuses.Draft : request.Status,
            ReservationExpiresAt = request.ReservationExpiresAt,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

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
            UnitCode = unit.UnitCode,
            CustomerName = customer.FullName,
            CustomerEmail = customer.Email,
            CustomerPhone = customer.PhoneNumber,
            AgentCode = agentCode
        };
    }
}
