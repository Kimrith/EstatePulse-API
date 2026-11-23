using EstatePulse.Modules.Bookings.DTOs.Booking;
using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Update;

public sealed record UpdateBookingCommand(
    int Id,
    int? AgentId,
    decimal? AgreedPrice,
    decimal? DiscountAmount,
    decimal? DepositRequired,
    string? Status,
    DateTime? ReservationExpiresAt,
    string? Notes
) : IRequest<BookingDto?>;
