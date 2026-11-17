using EstatePulse.Data;
using EstatePulse.DTOs;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EstatePulse.Queries
{
    public class AgentLeadQueries
    {
        // 1. Query to get paginated list of leads with optional filters
        public class GetPaginatedAgentLeadsQuery : IRequest<PaginatedList<AgentLeadDto>>
        {
            public int? AgentId { get; set; }
            public string? Status { get; set; }
            public string? SearchTerm { get; set; }
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }

        public class GetPaginatedAgentLeadsQueryHandler : IRequestHandler<GetPaginatedAgentLeadsQuery, PaginatedList<AgentLeadDto>>
        {
            private readonly AppDbContext _context;
            private readonly IMemoryCache _cache;
            private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

            public GetPaginatedAgentLeadsQueryHandler(AppDbContext context, IMemoryCache cache)
            {
                _context = context;
                _cache = cache;
            }

            public ValueTask<PaginatedList<AgentLeadDto>> Handle(GetPaginatedAgentLeadsQuery request, CancellationToken cancellationToken)
            {
                int pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
                int pageSize = request.PageSize < 1 ? 10 : request.PageSize;

                var searchKey = string.IsNullOrWhiteSpace(request.SearchTerm) ? "none" : request.SearchTerm.Trim().ToLowerInvariant();
                var statusKey = string.IsNullOrWhiteSpace(request.Status) ? "all" : request.Status.Trim().ToLowerInvariant();
                var agentKey = request.AgentId?.ToString() ?? "all";
                var cacheKey = $"agentleads-agent-{agentKey}-status-{statusKey}-search-{searchKey}-page-{pageNumber}-size-{pageSize}";

                // Fast path: synchronous return from cache
                if (_cache.TryGetValue(cacheKey, out PaginatedList<AgentLeadDto>? cachedList) && cachedList != null)
                {
                    return new ValueTask<PaginatedList<AgentLeadDto>>(cachedList);
                }

                return new ValueTask<PaginatedList<AgentLeadDto>>(FetchAndCacheAsync());

                async Task<PaginatedList<AgentLeadDto>> FetchAndCacheAsync()
                {
                    var query = _context.AgentLeads
                        .AsNoTracking()
                        .Include(l => l.Agent)
                        .Include(l => l.Customer)
                        .AsQueryable();

                    if (request.AgentId.HasValue)
                    {
                        query = query.Where(l => l.AgentId == request.AgentId.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(request.Status))
                    {
                        query = query.Where(l => l.Status == request.Status);
                    }

                    if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                    {
                        var term = request.SearchTerm.Trim().ToLower();
                        query = query.Where(l =>
                            l.LeadName.ToLower().Contains(term) ||
                            l.LeadPhone.Contains(term) ||
                            (l.LeadEmail != null && l.LeadEmail.ToLower().Contains(term)));
                    }

                    var totalCount = await query.CountAsync(cancellationToken);

                    var items = await query
                        .OrderByDescending(l => l.CreatedAt)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(l => new AgentLeadDto
                        {
                            Id = l.Id,
                            AgentId = l.AgentId,
                            CustomerId = l.CustomerId,
                            LeadName = l.LeadName,
                            LeadPhone = l.LeadPhone,
                            LeadEmail = l.LeadEmail,
                            BudgetMin = l.BudgetMin,
                            BudgetMax = l.BudgetMax,
                            Status = l.Status,
                            Notes = l.Notes,
                            CreatedAt = l.CreatedAt,
                            UpdatedAt = l.UpdatedAt,
                            AgentCode = l.Agent.AgentCode,
                            CustomerName = l.Customer != null ? l.Customer.FullName : null
                        })
                        .ToListAsync(cancellationToken);

                    var result = new PaginatedList<AgentLeadDto>(items, totalCount, pageNumber, pageSize);
                    _cache.Set(cacheKey, result, CacheDuration);
                    return result;
                }
            }
        }

        // 2. Query to get a single lead by ID
        public class GetAgentLeadByIdQuery : IRequest<AgentLeadDto?>
        {
            public int Id { get; set; }
        }

        public class GetAgentLeadByIdQueryHandler : IRequestHandler<GetAgentLeadByIdQuery, AgentLeadDto?>
        {
            private readonly AppDbContext _context;
            private readonly IMemoryCache _cache;
            private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

            public GetAgentLeadByIdQueryHandler(AppDbContext context, IMemoryCache cache)
            {
                _context = context;
                _cache = cache;
            }

            public ValueTask<AgentLeadDto?> Handle(GetAgentLeadByIdQuery request, CancellationToken cancellationToken)
            {
                var cacheKey = $"agentlead-id-{request.Id}";

                // Fast path: synchronous return from cache
                if (_cache.TryGetValue(cacheKey, out AgentLeadDto? cachedLead))
                {
                    return new ValueTask<AgentLeadDto?>(cachedLead);
                }

                return new ValueTask<AgentLeadDto?>(FetchAndCacheAsync());

                async Task<AgentLeadDto?> FetchAndCacheAsync()
                {
                    var lead = await _context.AgentLeads
                        .AsNoTracking()
                        .Include(l => l.Agent)
                        .Include(l => l.Customer)
                        .Where(l => l.Id == request.Id)
                        .Select(l => new AgentLeadDto
                        {
                            Id = l.Id,
                            AgentId = l.AgentId,
                            CustomerId = l.CustomerId,
                            LeadName = l.LeadName,
                            LeadPhone = l.LeadPhone,
                            LeadEmail = l.LeadEmail,
                            BudgetMin = l.BudgetMin,
                            BudgetMax = l.BudgetMax,
                            Status = l.Status,
                            Notes = l.Notes,
                            CreatedAt = l.CreatedAt,
                            UpdatedAt = l.UpdatedAt,
                            AgentCode = l.Agent.AgentCode,
                            CustomerName = l.Customer != null ? l.Customer.FullName : null
                        })
                        .FirstOrDefaultAsync(cancellationToken);

                    if (lead != null)
                    {
                        _cache.Set(cacheKey, lead, CacheDuration);
                    }

                    return lead;
                }
            }
        }
    }
}
