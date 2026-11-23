using Mediator;

namespace EstatePulse.Modules.Projects.Commands.Delete;

public sealed record DeleteProjectPhaseCommand(int Id) : IRequest<bool>;
