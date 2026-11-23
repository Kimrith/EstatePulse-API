using EstatePulse.Modules.Users.DTOs;
using Mediator;

namespace EstatePulse.Modules.Users.Queries;

public sealed record GetUserByIdQuery(int Id) : IRequest<UserDto?>;
