using EstatePulse.Modules.Bookings.DTOs.Commissions;
using Mediator;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed record GetCommissionByIdQuery(int Id) : IRequest<CommissionDto?>;
