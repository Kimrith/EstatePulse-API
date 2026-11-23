using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Queries;

public sealed class GetUnitMediaByIdQueryHandler : IRequestHandler<GetUnitMediaByIdQuery, UnitMediaDto?>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public GetUnitMediaByIdQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<UnitMediaDto?> Handle(GetUnitMediaByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"unit-media-id-{request.Id}";

        if (_cache.TryGet(cacheKey, out UnitMediaDto? cached))
        {
            return new ValueTask<UnitMediaDto?>(cached);
        }

        return new ValueTask<UnitMediaDto?>(FetchAndCacheAsync());

        async Task<UnitMediaDto?> FetchAndCacheAsync()
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
                WHERE um.[id] = @id;";

            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = request.Id });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var dto = UnitMediaQueryMapping.ReadUnitMediaDto(reader);
                _cache.Set(cacheKey, dto, CacheDuration, prefix: "unit-media");
                return dto;
            }

            return null;
        }
    }
}
