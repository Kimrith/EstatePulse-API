using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Agents.DTOs;
using EstatePulse.Modules.Agents.DTOs.Agent;
using EstatePulse.Modules.Agents.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Agents.Commands.Create;

public sealed class CreateAgentCommandHandler : IRequestHandler<CreateAgentCommand, AgentDto>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;

    public CreateAgentCommandHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async ValueTask<AgentDto> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
    {
        // Check if the user exists
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            throw new InvalidOperationException($"No user found with ID '{request.UserId}'.");
        }

        var agent = new Agent
        {
            UserId = request.UserId,
            AgentCode = request.AgentCode,
            LicenseNumber = request.LicenseNumber,
            CommissionRate = request.CommissionRate,
            AssignedBranch = request.AssignedBranch,
            TotalDealsClosed = request.TotalDealsClosed,
            Rating = request.Rating,
            Bio = request.Bio,
            IsVerified = request.IsVerified,
            CreatedAt = DateTime.UtcNow
        };

        _context.Agents.Add(agent);
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
            CreatedAt = agent.CreatedAt
        };
    }
}
