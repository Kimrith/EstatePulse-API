using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Queries;

public sealed class GetUnitTypeByIdQueryHandler : IRequestHandler<GetUnitTypeByIdQuery, UnitTypeDto?>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public GetUnitTypeByIdQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<UnitTypeDto?> Handle(GetUnitTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"unit-type-id-{request.Id}";

        if (_cache.TryGet(cacheKey, out UnitTypeDto? cached))
        {
            return new ValueTask<UnitTypeDto?>(cached);
        }

        return new ValueTask<UnitTypeDto?>(FetchAndCacheAsync());

        async Task<UnitTypeDto?> FetchAndCacheAsync()
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    ut.[id],
                    ut.[type_name],
                    ut.[code],
                    ut.[bedrooms],
                    ut.[bathrooms],
                    ut.[parking_slots],
                    ut.[floors],
                    ut.[building_size],
                    ut.[land_size_recommended],
                    ut.[floor_plan_image_url],
                    ut.[specification_json],
                    (SELECT COUNT(1) FROM [Units] u WHERE u.[unit_type_id] = ut.[id]) AS [total_units_count]
                FROM [UnitTypes] ut
                WHERE ut.[id] = @id;";

            command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = request.Id });

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var dto = UnitTypeQueryMapping.ReadUnitTypeDto(reader);
                _cache.Set(cacheKey, dto, CacheDuration, prefix: "unit-types");
                return dto;
            }

            return null;
        }
    }
}
