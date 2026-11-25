using EstatePulse.Common.Results;
using EstatePulse.Modules.Bookings.DTOs.Commissions;
using Mediator;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed record GetPaginatedCommissionsQuery(
    int? BookingId = null,
    int? AgentId = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<CommissionDto>>;
