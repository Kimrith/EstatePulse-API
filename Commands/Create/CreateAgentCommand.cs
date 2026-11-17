using EstatePulse.Data;
using EstatePulse.DTOs;
using EstatePulse.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Commands.Create
{
    public class CreateAgentCommand : IRequest<AgentDto>
    {
        public AgentCreateDto Agent { get; set; } = new AgentCreateDto();
    }

    public class CreateAgentCommandHandler : IRequestHandler<CreateAgentCommand, AgentDto>
    {
        private readonly AppDbContext _context;

        public CreateAgentCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<AgentDto> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Agent;

            // Check if the user exists
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == dto.UserId, cancellationToken);

            if (!userExists)
            {
                throw new InvalidOperationException($"No user found with ID '{dto.UserId}'.");
            }

            var agent = new Agent
            {
                UserId = dto.UserId,
                AgentCode = dto.AgentCode,
                LicenseNumber = dto.LicenseNumber,
                CommissionRate = dto.CommissionRate,
                AssignedBranch = dto.AssignedBranch,
                TotalDealsClosed = dto.TotalDealsClosed,
                Rating = dto.Rating,
                Bio = dto.Bio,
                IsVerified = dto.IsVerified,
                CreatedAt = DateTime.UtcNow
            };

            _context.Agents.Add(agent);
            await _context.SaveChangesAsync(cancellationToken);

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
                CreatedAt = agent.CreatedAt
            };
        }
    }
}