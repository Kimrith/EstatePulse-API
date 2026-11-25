using EstatePulse.Modules.Bookings.DTOs.Contract;
using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Create;

public sealed record CreateContractCommand(
    int BookingId,
    string? ContractNumber,
    string? DocumentUrl,
    string? Status
) : IRequest<ContractDto>;
