using Mediator;

namespace EstatePulse.Modules.Users.Commands.Delete;

public sealed record DeleteUserCommand(int Id) : IRequest<bool>;
