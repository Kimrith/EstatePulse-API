using Mediator;

namespace EstatePulse.Modules.Users.Queries;

public sealed record IsFollowingQuery(int FollowerId, int FollowedId) : IRequest<bool>;
