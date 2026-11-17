using EstatePulse.Data;
using EstatePulse.DTOs;
using Mediator;

namespace EstatePulse.Commands.Update
{
    public class UpdateAgentCommand : IRequest<AgentDto?>
    {
        public int Id { get; set; }
        public AgentUpdateDto Agent { get; set; } = new();
    }

    public class UpdateAgentCommandHandler : IRequestHandler<UpdateAgentCommand, AgentDto?>
    {
        private readonly AppDbContext _context;

        public UpdateAgentCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<AgentDto?> Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
        {
            var agent = await _context.Agents.FindAsync(new object[] { request.Id }, cancellationToken);
            if (agent == null)
            {
                return null; // Not found
            }

            var dto = request.Agent;

            // Update fields only if they are provided in the DTO
            if (dto.LicenseNumber != null) agent.LicenseNumber = dto.LicenseNumber;
            if (dto.CommissionRate.HasValue) agent.CommissionRate = dto.CommissionRate.Value;
            if (dto.AssignedBranch != null) agent.AssignedBranch = dto.AssignedBranch;
            if (dto.TotalDealsClosed.HasValue) agent.TotalDealsClosed = dto.TotalDealsClosed.Value;
            if (dto.Rating.HasValue) agent.Rating = dto.Rating.Value;
            if (dto.Bio != null) agent.Bio = dto.Bio;
            if (dto.IsVerified.HasValue) agent.IsVerified = dto.IsVerified.Value;

            agent.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // Return the updated AgentDto
            return new AgentDto
            {
                Id = agent.Id,
                UserId = agent.UserId,
                AgentCode = agent.AgentCode,
                LicenseNumber = agent.LicenseNumber,
                CommissionRate = agent.CommissionRate,
                AssignedBranch = agent.AssignedBranch,
                TotalDealsClosed = agent.TotalDealsClosed,
                Rating = agent.Rating,
                Bio = agent.Bio,
                IsVerified = agent.IsVerified,
                CreatedAt = agent.CreatedAt,
                UpdatedAt = agent.UpdatedAt
            };
        }
    }
}