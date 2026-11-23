using EstatePulse.Modules.Bookings.DTOs.Booking;
using Mediator;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed record GetBookingByIdQuery(int Id) : IRequest<BookingDto?>;
