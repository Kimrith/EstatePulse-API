using EstatePulse.Common.Database;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Bookings.Commands.Delete;

public sealed class DeleteBookingCommandHandler : IRequestHandler<DeleteBookingCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteBookingCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<bool> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            return false;
        }

        var hasPayments = await _context.Payments
            .AnyAsync(p => p.BookingId == request.Id, cancellationToken);

        if (hasPayments)
        {
            throw new InvalidOperationException($"Cannot delete booking '{booking.BookingReference}' because it has associated payment records.");
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
