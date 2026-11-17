using EstatePulse.Data;
using EstatePulse.DTOs;
using EstatePulse.Models;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EstatePulse.Queries
{
    public class AgentQueries
    {
        // 1. Query to get paginated list of agents using Raw SQL
        public class GetPaginatedAgentsQuery : IRequest<PaginatedList<AgentDto>>
        {
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }

        public class GetPaginatedAgentsQueryHandler : IRequestHandler<GetPaginatedAgentsQuery, PaginatedList<AgentDto>>
        {
            private readonly AppDbContext _context;
            private readonly IMemoryCache _cache;
            private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

            public GetPaginatedAgentsQueryHandler(AppDbContext context, IMemoryCache cache)
            {
                _context = context;
                _cache = cache;
            }

            public ValueTask<PaginatedList<AgentDto>> Handle(GetPaginatedAgentsQuery request, CancellationToken cancellationToken)
            {
                // Ensure page number and size are valid
                int pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
                int pageSize = request.PageSize < 1 ? 10 : request.PageSize;
                var cacheKey = $"agents-page-{pageNumber}-size-{pageSize}";

                // Fast path: synchronous return from cache
                if (_cache.TryGetValue(cacheKey, out PaginatedList<AgentDto>? cachedList) && cachedList != null)
                {
                    return new ValueTask<PaginatedList<AgentDto>>(cachedList);
                }

                return new ValueTask<PaginatedList<AgentDto>>(FetchAndCacheAsync());

                async Task<PaginatedList<AgentDto>> FetchAndCacheAsync()
                {
                    int offset = (pageNumber - 1) * pageSize;

                    // Get total count using raw SQL
                    var countSql = "SELECT COUNT(*) AS Value FROM Agents";
                    var totalCount = await _context.Database
                        .SqlQueryRaw<int>(countSql)
                        .SingleAsync(cancellationToken);

                    // Get paginated slice using raw SQL
                    var dataSql = $"SELECT id, user_id, agent_code, license_number, commission_rate, assigned_branch, total_deals_closed, rating, bio, is_verified, created_at, updated_at FROM Agents ORDER BY created_at DESC OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
                    var agents = await _context.Agents
                        .FromSqlRaw(dataSql)
                        .ToListAsync(cancellationToken);

                    // Map models to DTOs
                    var agentDtos = agents.Select(agent => new AgentDto
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
                    }).ToList();

                    var result = new PaginatedList<AgentDto>(agentDtos, totalCount, pageNumber, pageSize);
                    _cache.Set(cacheKey, result, CacheDuration);
                    return result;
                }
            }
        }

        // 2. Query to get a single agent by ID using Raw SQL
        public class GetAgentByIdQuery : IRequest<AgentDto?>
        {
            public int Id { get; set; }
        }

        public class GetAgentByIdQueryHandler : IRequestHandler<GetAgentByIdQuery, AgentDto?>
        {
            private readonly AppDbContext _context;
            private readonly IMemoryCache _cache;
            private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

            public GetAgentByIdQueryHandler(AppDbContext context, IMemoryCache cache)
            {
                _context = context;
                _cache = cache;
            }

            public ValueTask<AgentDto?> Handle(GetAgentByIdQuery request, CancellationToken cancellationToken)
            {
                var cacheKey = $"agent-id-{request.Id}";

                // Fast path: synchronous return from cache
                if (_cache.TryGetValue(cacheKey, out AgentDto? cachedAgent))
                {
                    return new ValueTask<AgentDto?>(cachedAgent);
                }

                return new ValueTask<AgentDto?>(FetchAndCacheAsync());

                async Task<AgentDto?> FetchAndCacheAsync()
                {
                    var sql = "SELECT id, user_id, agent_code, license_number, commission_rate, assigned_branch, total_deals_closed, rating, bio, is_verified, created_at, updated_at FROM Agents WHERE id = {0}";

                    var agent = await _context.Agents
                        .FromSqlRaw(sql, request.Id)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (agent == null) return null;

                    var dto = new AgentDto
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

                    _cache.Set(cacheKey, dto, CacheDuration);
                    return dto;
                }
            }
        }
    }
}