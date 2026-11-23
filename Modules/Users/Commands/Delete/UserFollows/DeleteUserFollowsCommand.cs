using Mediator;

namespace EstatePulse.Modules.Users.Commands.Delete;

public sealed record DeleteUserFollowsCommand(int Id) : IRequest<bool>;
