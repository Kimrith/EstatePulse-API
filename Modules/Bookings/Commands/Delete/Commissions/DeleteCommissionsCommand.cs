using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Delete;

public sealed record DeleteCommissionsCommand(int Id) : IRequest<bool>;
