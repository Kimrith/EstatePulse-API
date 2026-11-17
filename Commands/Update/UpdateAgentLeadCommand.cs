using EstatePulse.Data;
using EstatePulse.DTOs;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Commands.Update
{
    public class UpdateAgentLeadCommand : IRequest<AgentLeadDto?>
    {
        public int Id { get; set; }
        public AgentLeadUpdateDto Lead { get; set; } = new();
    }

    public class UpdateAgentLeadCommandHandler : IRequestHandler<UpdateAgentLeadCommand, AgentLeadDto?>
    {
        private readonly AppDbContext _context;

        public UpdateAgentLeadCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async ValueTask<AgentLeadDto?> Handle(UpdateAgentLeadCommand request, CancellationToken cancellationToken)
        {
            var lead = await _context.AgentLeads
                .Include(l => l.Agent)
                .Include(l => l.Customer)
                .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

            if (lead == null)
            {
                return null;
            }

            var dto = request.Lead;

            if (dto.CustomerId.HasValue)
            {
                var customer = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.CustomerId.Value, cancellationToken);

                if (customer == null)
                {
                    throw new InvalidOperationException($"No customer found with ID '{dto.CustomerId.Value}'.");
                }

                lead.CustomerId = dto.CustomerId.Value;
                lead.Customer = customer;
            }

            if (dto.LeadName != null) lead.LeadName = dto.LeadName;
            if (dto.LeadPhone != null) lead.LeadPhone = dto.LeadPhone;
            if (dto.LeadEmail != null) lead.LeadEmail = dto.LeadEmail;
            if (dto.BudgetMin.HasValue) lead.BudgetMin = dto.BudgetMin.Value;
            if (dto.BudgetMax.HasValue) lead.BudgetMax = dto.BudgetMax.Value;
            if (!string.IsNullOrWhiteSpace(dto.Status)) lead.Status = dto.Status;
            if (dto.Notes != null) lead.Notes = dto.Notes;

            lead.UpdatedAt = DateTime.UtcNow;

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
                AgentCode = lead.Agent?.AgentCode,
                CustomerName = lead.Customer?.FullName
            };
        }
    }
}
