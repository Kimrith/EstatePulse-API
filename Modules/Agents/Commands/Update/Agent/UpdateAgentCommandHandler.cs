using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Agents.DTOs;
using EstatePulse.Modules.Agents.DTOs.Agent;
using Mediator;

namespace EstatePulse.Modules.Agents.Commands.Update;

public sealed class UpdateAgentCommandHandler : IRequestHandler<UpdateAgentCommand, AgentDto?>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;

    public UpdateAgentCommandHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async ValueTask<AgentDto?> Handle(UpdateAgentCommand request, CancellationToken cancellationToken)
    {
        var agent = await _context.Agents.FindAsync(new object[] { request.Id }, cancellationToken);
        if (agent == null)
        {
            return null;
        }

        if (request.LicenseNumber != null) agent.LicenseNumber = request.LicenseNumber;
        if (request.CommissionRate.HasValue) agent.CommissionRate = request.CommissionRate.Value;
        if (request.AssignedBranch != null) agent.AssignedBranch = request.AssignedBranch;
        if (request.TotalDealsClosed.HasValue) agent.TotalDealsClosed = request.TotalDealsClosed.Value;
        if (request.Rating.HasValue) agent.Rating = request.Rating.Value;
        if (request.Bio != null) agent.Bio = request.Bio;
        if (request.IsVerified.HasValue) agent.IsVerified = request.IsVerified.Value;

        agent.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        _cache.RemoveByPrefix("agents");

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
