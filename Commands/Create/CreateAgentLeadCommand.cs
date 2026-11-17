using EstatePulse.Data;
using EstatePulse.DTOs;
using EstatePulse.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Commands.Create
{
    public class CreateAgentLeadCommand : IRequest<AgentLeadDto>
    {
        public AgentLeadCreateDto Lead { get; set; } = new();
    }

    public class CreateAgentLeadCommandHandler : IRequestHandler<CreateAgentLeadCommand, AgentLeadDto>
    {
        private readonly AppDbContext _context;

        public CreateAgentLeadCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<AgentLeadDto> Handle(CreateAgentLeadCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Lead;

            // 1. Verify agent exists
            var agent = await _context.Agents
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == dto.AgentId, cancellationToken);

            if (agent == null)
            {
                throw new InvalidOperationException($"No agent found with ID '{dto.AgentId}'.");
            }

            // 2. If customer_id provided, verify customer user exists
            string? customerName = null;
            if (dto.CustomerId.HasValue)
            {
                var customer = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.CustomerId.Value, cancellationToken);

                if (customer == null)
                {
                    throw new InvalidOperationException($"No customer found with ID '{dto.CustomerId.Value}'.");
                }

                customerName = customer.FullName;
            }

            var lead = new AgentLead
            {
                AgentId = dto.AgentId,
                CustomerId = dto.CustomerId,
                LeadName = dto.LeadName,
                LeadPhone = dto.LeadPhone,
                LeadEmail = dto.LeadEmail,
                BudgetMin = dto.BudgetMin,
                BudgetMax = dto.BudgetMax,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? AgentLead.Statuses.New : dto.Status,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.AgentLeads.Add(lead);
            await _context.SaveChangesAsync(cancellationToken);

            return new AgentLeadDto
            {
                Id = lead.Id,
                AgentId = lead.AgentId,
                CustomerId = lead.CustomerId,
                LeadName = lead.LeadName,
                LeadPhone = lead.LeadPhone,
                LeadEmail = lead.LeadEmail,
                BudgetMin = lead.BudgetMin,
                BudgetMax = lead.BudgetMax,
                Status = lead.Status,
                Notes = lead.Notes,
                CreatedAt = lead.CreatedAt,
                UpdatedAt = lead.UpdatedAt,
                AgentCode = agent.AgentCode,
                CustomerName = customerName
            };
        }
    }
}
