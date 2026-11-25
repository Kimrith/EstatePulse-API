using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Delete;

public sealed record DeleteContractCommand(int Id) : IRequest<bool>;
