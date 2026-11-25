using EstatePulse.Modules.Bookings.DTOs.Contract;
using Mediator;

namespace EstatePulse.Modules.Bookings.Queries;

public sealed record GetContractByIdQuery(int Id) : IRequest<ContractDto?>;
