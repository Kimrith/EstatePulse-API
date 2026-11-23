using EstatePulse.Modules.Bookings.DTOs.Booking;
using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Update;

public sealed record UpdateBookingStatusCommand(int Id, string Status) : IRequest<BookingDto?>;
