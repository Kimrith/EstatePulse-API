using EstatePulse.Modules.Users.DTOs;
using Mediator;

namespace EstatePulse.Modules.Users.Queries;

public sealed record GetUserByEmailQuery(string Email) : IRequest<UserDto?>;
