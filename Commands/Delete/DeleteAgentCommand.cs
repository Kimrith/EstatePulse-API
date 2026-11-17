using EstatePulse.Data;
using Mediator;

namespace EstatePulse.Commands.Delete
{
    public record DeleteAgentCommand(int Id) : IRequest<bool>;

    public class DeleteAgentCommandHandler : IRequestHandler<DeleteAgentCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteAgentCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<bool> Handle(DeleteAgentCommand request, CancellationToken cancellationToken)
        {
            // Find the agent by ID
            var agent = await _context.Agents.FindAsync(new object[] { request.Id }, cancellationToken);

            if (agent == null)
            {
                return false; // Agent not found
            }

            // Remove from context and save changes
            _context.Agents.Remove(agent);
            await _context.SaveChangesAsync(cancellationToken);

            return true; // Successfully deleted
        }
    }
}