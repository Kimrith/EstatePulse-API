using EstatePulse.Common.Database;
using EstatePulse.Modules.Agents.DTOs;
using EstatePulse.Modules.Agents.DTOs.AgentLead;
using EstatePulse.Modules.Agents.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Agents.Commands.Create;

public sealed class CreateAgentLeadCommandHandler : IRequestHandler<CreateAgentLeadCommand, AgentLeadDto>
{
    private readonly AppDbContext _context;

    public CreateAgentLeadCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<AgentLeadDto> Handle(CreateAgentLeadCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify agent exists
        var agent = await _context.Agents
            .FirstOrDefaultAsync(a => a.Id == request.AgentId, cancellationToken);

        if (agent == null)
        {
            throw new InvalidOperationException($"No agent found with ID '{request.AgentId}'.");
        }

        // 2. If customer_id provided, verify customer user exists
        string? customerName = null;
        if (request.CustomerId.HasValue)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.CustomerId.Value, cancellationToken);

            if (customer == null)
            {
                throw new InvalidOperationException($"No customer found with ID '{request.CustomerId.Value}'.");
            }

            customerName = customer.FullName;
        }

        var lead = new AgentLead
        {
            AgentId = request.AgentId,
            CustomerId = request.CustomerId,
            LeadName = request.LeadName,
            LeadPhone = request.LeadPhone,
            LeadEmail = request.LeadEmail,
            BudgetMin = request.BudgetMin,
            BudgetMax = request.BudgetMax,
            Status = string.IsNullOrWhiteSpace(request.Status) ? AgentLead.Statuses.New : request.Status,
            Notes = request.Notes,
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
