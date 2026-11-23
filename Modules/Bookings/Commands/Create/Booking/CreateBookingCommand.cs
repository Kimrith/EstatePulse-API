using EstatePulse.Modules.Bookings.DTOs.Booking;
using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Create;

public sealed record CreateBookingCommand(
    int UnitId,
    int CustomerId,
    int? AgentId,
    decimal AgreedPrice,
    decimal DiscountAmount,
    decimal DepositRequired,
    string? Status,
    DateTime? ReservationExpiresAt,
    string? Notes
) : IRequest<BookingDto>;
