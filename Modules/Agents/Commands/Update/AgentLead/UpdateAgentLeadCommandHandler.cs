using EstatePulse.Common.Database;
using EstatePulse.Modules.Agents.DTOs.AgentLead;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Agents.Commands.Update;

public sealed class UpdateAgentLeadCommandHandler : IRequestHandler<UpdateAgentLeadCommand, AgentLeadDto?>
{
    private readonly AppDbContext _context;

    public UpdateAgentLeadCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<AgentLeadDto?> Handle(UpdateAgentLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await _context.AgentLeads
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (lead == null)
        {
            return null;
        }

        string? customerName = null;
        if (request.CustomerId.HasValue)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.CustomerId.Value, cancellationToken);

            if (customer == null)
            {
                throw new InvalidOperationException($"No customer found with ID '{request.CustomerId.Value}'.");
            }

            lead.CustomerId = request.CustomerId.Value;
            customerName = customer.FullName;
        }
        else if (lead.CustomerId.HasValue)
        {
            customerName = await _context.Users
                .Where(u => u.Id == lead.CustomerId.Value)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (request.LeadName != null) lead.LeadName = request.LeadName;
        if (request.LeadPhone != null) lead.LeadPhone = request.LeadPhone;
        if (request.LeadEmail != null) lead.LeadEmail = request.LeadEmail;
        if (request.BudgetMin.HasValue) lead.BudgetMin = request.BudgetMin.Value;
        if (request.BudgetMax.HasValue) lead.BudgetMax = request.BudgetMax.Value;
        if (!string.IsNullOrWhiteSpace(request.Status)) lead.Status = request.Status;
        if (request.Notes != null) lead.Notes = request.Notes;

        lead.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var agentCode = await _context.Agents
            .Where(a => a.Id == lead.AgentId)
            .Select(a => a.AgentCode)
            .FirstOrDefaultAsync(cancellationToken);

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
            AgentCode = agentCode,
            CustomerName = customerName
        };
    }
}
