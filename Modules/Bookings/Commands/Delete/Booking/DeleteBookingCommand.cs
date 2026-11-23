using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Delete;

public sealed record DeleteBookingCommand(int Id) : IRequest<bool>;
