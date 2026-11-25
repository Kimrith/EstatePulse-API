using EstatePulse.Modules.Bookings.DTOs.Contract;
using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Update;

public sealed record UpdateContractStatusCommand(
    int Id,
    string Status
) : IRequest<ContractDto?>;
