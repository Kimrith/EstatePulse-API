using System.Data;
using EstatePulse.Common.Caching;
using EstatePulse.Common.Database;
using EstatePulse.Common.Results;
using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Queries;

public sealed class GetUnitTypesQueryHandler : IRequestHandler<GetUnitTypesQuery, PaginatedList<UnitTypeDto>>
{
    private readonly AppDbContext _context;
    private readonly ICacheService _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public GetUnitTypesQueryHandler(AppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public ValueTask<PaginatedList<UnitTypeDto>> Handle(GetUnitTypesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        var cacheKey = $"unit-types-page-{pageNumber}-size-{pageSize}";

        if (_cache.TryGet(cacheKey, out PaginatedList<UnitTypeDto>? cachedList) && cachedList != null)
        {
            return new ValueTask<PaginatedList<UnitTypeDto>>(cachedList);
        }

        return new ValueTask<PaginatedList<UnitTypeDto>>(FetchAndCacheAsync());

        async Task<PaginatedList<UnitTypeDto>> FetchAndCacheAsync()
        {
            var offset = (pageNumber - 1) * pageSize;
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT 
                    COUNT(*) OVER() AS [TotalCount],
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
                ORDER BY ut.[created_at] DESC
                OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
            command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

            int totalCount = 0;
            var items = new List<UnitTypeDto>(pageSize);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                if (totalCount == 0)
                {
                    totalCount = reader.GetInt32(0);
                }

                items.Add(UnitTypeQueryMapping.ReadUnitTypeDto(reader, startIndex: 1));
            }

            if (totalCount == 0 && offset > 0)
            {
                await using var countCmd = connection.CreateCommand();
                countCmd.CommandText = "SELECT COUNT(1) FROM [UnitTypes];";
                var scalar = await countCmd.ExecuteScalarAsync(cancellationToken);
                totalCount = Convert.ToInt32(scalar);
            }

            var result = new PaginatedList<UnitTypeDto>(items, totalCount, pageNumber, pageSize);
            _cache.Set(cacheKey, result, CacheDuration, prefix: "unit-types");
            return result;
        }
    }
}
