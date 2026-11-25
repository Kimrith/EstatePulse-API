using EstatePulse.Common.Results;
using EstatePulse.Modules.Bookings.DTOs.Contract;
using Mediator;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed record GetPaginatedContractsQuery(
    int? BookingId = null,
    string? Status = null,
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<ContractDto>>;
