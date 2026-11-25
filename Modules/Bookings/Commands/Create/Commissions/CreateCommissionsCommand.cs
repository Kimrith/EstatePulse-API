using EstatePulse.Modules.Bookings.DTOs.Commissions;
using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Create;

public sealed record CreateCommissionsCommand(
    int BookingId,
    int AgentId,
    decimal CommissionRate,
    decimal? CommissionAmount,
    string? Status
) : IRequest<CommissionDto>;
