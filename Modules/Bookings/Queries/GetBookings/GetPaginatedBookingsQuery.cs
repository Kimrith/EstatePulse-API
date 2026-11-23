using EstatePulse.Common.Results;
using EstatePulse.Modules.Bookings.DTOs.Booking;
using Mediator;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed record GetPaginatedBookingsQuery(
    int? UnitId = null,
    int? CustomerId = null,
    int? AgentId = null,
    string? Status = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<BookingDto>>;
