using Mediator;

namespace EstatePulse.Modules.Projects.Commands.Delete;

public sealed record DeleteProjectCommand(int Id) : IRequest<bool>;
