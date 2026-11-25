using EstatePulse.Modules.Bookings.DTOs.Commissions;
using Mediator;

namespace EstatePulse.Modules.Bookings.Commands.Update;

public sealed record UpdateCommissionCommand(
    int Id,
    decimal? CommissionRate,
    decimal? CommissionAmount,
    string? Status,
    int? ApprovedByUserId,
    DateTime? ApprovedAt,
    DateTime? PaidAt
) : IRequest<CommissionDto?>;
