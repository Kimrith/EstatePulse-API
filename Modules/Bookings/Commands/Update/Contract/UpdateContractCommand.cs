using EstatePulse.Modules.Bookings.DTOs.Contract;
using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Update;

public sealed record UpdateContractCommand(
    int Id,
    string? ContractNumber,
    string? DocumentUrl,
    string? Status,
    DateTime? SignedAt
) : IRequest<ContractDto?>;
