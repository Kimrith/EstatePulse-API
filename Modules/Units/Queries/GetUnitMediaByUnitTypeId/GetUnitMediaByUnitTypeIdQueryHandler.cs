using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Queries;

public sealed class GetUnitMediaByUnitTypeIdQueryHandler : IRequestHandler<GetUnitMediaByUnitTypeIdQuery, List<UnitMediaDto>>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public GetUnitMediaByUnitTypeIdQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<List<UnitMediaDto>> Handle(GetUnitMediaByUnitTypeIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"unit-media-type-{request.UnitTypeId}";

        if (_cache.TryGet(cacheKey, out List<UnitMediaDto>? cachedList) && cachedList != null)
        {
            return new ValueTask<List<UnitMediaDto>>(cachedList);
        }

        return new ValueTask<List<UnitMediaDto>>(FetchAndCacheAsync());

        async Task<List<UnitMediaDto>> FetchAndCacheAsync()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    um.[id],
                    um.[unit_id],
                    u.[unit_code],
                    um.[unit_type_id],
                    ut.[code] AS [unit_type_code],
                    um.[media_type],
                    um.[media_url],
                    um.[sort_order],
                    um.[is_featured]
                FROM [UnitMedia] um
                LEFT JOIN [Units] u ON u.[id] = um.[unit_id]
                LEFT JOIN [UnitTypes] ut ON ut.[id] = um.[unit_type_id]
                WHERE um.[unit_type_id] = @unitTypeId
                ORDER BY um.[sort_order] ASC, um.[id] ASC;";

            command.Parameters.Add(new SqlParameter("@unitTypeId", SqlDbType.Int) { Value = request.UnitTypeId });

            var items = new List<UnitMediaDto>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                items.Add(UnitMediaQueryMapping.ReadUnitMediaDto(reader));
            }

            _cache.Set(cacheKey, items, CacheDuration, prefix: "unit-media");
            return items;
        }
    }
}
