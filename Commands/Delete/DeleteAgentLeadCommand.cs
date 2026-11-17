using EstatePulse.Data;
using Mediator;

namespace EstatePulse.Commands.Delete
{
    public record DeleteAgentLeadCommand(int Id) : IRequest<bool>;

    public class DeleteAgentLeadCommandHandler : IRequestHandler<DeleteAgentLeadCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteAgentLeadCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<bool> Handle(DeleteAgentLeadCommand request, CancellationToken cancellationToken)
        {
            var lead = await _context.AgentLeads.FindAsync(new object[] { request.Id }, cancellationToken);

            if (lead == null)
            {
                return false;
            }

            _context.AgentLeads.Remove(lead);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
