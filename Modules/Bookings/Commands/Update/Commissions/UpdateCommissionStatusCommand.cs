using EstatePulse.Modules.Bookings.DTOs.Commissions;
using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Update;

public sealed record UpdateCommissionStatusCommand(
    int Id,
    string Status,
    int? ApprovedByUserId = null
) : IRequest<CommissionDto?>;
