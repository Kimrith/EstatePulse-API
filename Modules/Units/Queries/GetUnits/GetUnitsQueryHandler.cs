using System.Data;
using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Common.Results;
using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Queries;

public sealed class GetUnitsQueryHandler : IRequestHandler<GetUnitsQuery, PaginatedList<UnitDto>>
{
    private readonly AppDbContext _context;

    public GetUnitsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<PaginatedList<UnitDto>> Handle(GetUnitsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        var offset = (pageNumber - 1) * pageSize;

        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();

        var utcNow = DateTime.UtcNow;
        bool isAvailableSearch = string.Equals(request.Status, "Available", StringComparison.OrdinalIgnoreCase);
        bool hasSearch = !string.IsNullOrWhiteSpace(request.SearchTerm);
        var searchPattern = hasSearch ? $"%{request.SearchTerm!.Trim()}%" : null;

        command.CommandText = @"
            SELECT 
                COUNT(*) OVER() AS [TotalCount],
                u.[id],
                u.[phase_id],
                pp.[phase_name],
                pp.[project_id],
                p.[name] AS [project_name],
                u.[unit_type_id],
                ut.[code] AS [unit_type_code],
                ut.[type_name] AS [unit_type_name],
                u.[unit_code],
                u.[street_number],
                u.[land_area_sqm],
                u.[built_area_sqm],
                u.[base_price],
                u.[status],
                u.[hold_expires_at],
                u.[current_hold_user_id],
                usr.[full_name] AS [current_hold_user_name],
                u.[svg_element_id],
                u.[row_version],
                u.[created_at],
                u.[updated_at]
            FROM [Units] u
            INNER JOIN [ProjectPhases] pp ON pp.[id] = u.[phase_id]
            INNER JOIN [Projects] p ON p.[id] = pp.[project_id]
            INNER JOIN [UnitTypes] ut ON ut.[id] = u.[unit_type_id]
            LEFT JOIN [Users] usr ON usr.[id] = u.[current_hold_user_id]
            WHERE (@phaseId IS NULL OR u.[phase_id] = @phaseId)
              AND (@projectId IS NULL OR pp.[project_id] = @projectId)
              AND (@unitTypeId IS NULL OR u.[unit_type_id] = @unitTypeId)
              AND (@status IS NULL 
                   OR (@isAvailableSearch = 1 AND (u.[status] = 'Available' OR (u.[status] = 'OnHold' AND u.[hold_expires_at] IS NOT NULL AND u.[hold_expires_at] <= @utcNow)))
                   OR (@isAvailableSearch = 0 AND u.[status] = @status))
              AND (@minPrice IS NULL OR u.[base_price] >= @minPrice)
              AND (@maxPrice IS NULL OR u.[base_price] <= @maxPrice)
              AND (@minBedrooms IS NULL OR ut.[bedrooms] >= @minBedrooms)
              AND (@hasSearch = 0 OR (u.[unit_code] LIKE @searchPattern OR (u.[street_number] IS NOT NULL AND u.[street_number] LIKE @searchPattern)))
            ORDER BY u.[created_at] DESC
            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

        command.Parameters.Add(new SqlParameter("@phaseId", SqlDbType.Int) { Value = (object?)request.PhaseId ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@projectId", SqlDbType.Int) { Value = (object?)request.ProjectId ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@unitTypeId", SqlDbType.Int) { Value = (object?)request.UnitTypeId ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@status", SqlDbType.NVarChar, 50) { Value = (object?)request.Status ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@isAvailableSearch", SqlDbType.Bit) { Value = isAvailableSearch });
        command.Parameters.Add(new SqlParameter("@utcNow", SqlDbType.DateTime2) { Value = utcNow });
        command.Parameters.Add(new SqlParameter("@minPrice", SqlDbType.Decimal) { Value = (object?)request.MinPrice ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@maxPrice", SqlDbType.Decimal) { Value = (object?)request.MaxPrice ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@minBedrooms", SqlDbType.Int) { Value = (object?)request.MinBedrooms ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@hasSearch", SqlDbType.Bit) { Value = hasSearch });
        command.Parameters.Add(new SqlParameter("@searchPattern", SqlDbType.NVarChar, 100) { Value = (object?)searchPattern ?? DBNull.Value });
        command.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int) { Value = offset });
        command.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) { Value = pageSize });

        int totalCount = 0;
        var items = new List<UnitDto>(pageSize);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            if (totalCount == 0)
            {
                totalCount = reader.GetInt32(0);
            }
            items.Add(UnitQueryMapping.ReadUnitDto(reader, utcNow, offset: 1));
        }

        if (totalCount == 0 && offset > 0)
        {
            await using var countCmd = connection.CreateCommand();
            countCmd.CommandText = @"
                SELECT COUNT(1)
                FROM [Units] u
                INNER JOIN [ProjectPhases] pp ON pp.[id] = u.[phase_id]
                INNER JOIN [Projects] p ON p.[id] = pp.[project_id]
                INNER JOIN [UnitTypes] ut ON ut.[id] = u.[unit_type_id]
                WHERE (@phaseId IS NULL OR u.[phase_id] = @phaseId)
                  AND (@projectId IS NULL OR pp.[project_id] = @projectId)
                  AND (@unitTypeId IS NULL OR u.[unit_type_id] = @unitTypeId)
                  AND (@status IS NULL 
                       OR (@isAvailableSearch = 1 AND (u.[status] = 'Available' OR (u.[status] = 'OnHold' AND u.[hold_expires_at] IS NOT NULL AND u.[hold_expires_at] <= @utcNow)))
                       OR (@isAvailableSearch = 0 AND u.[status] = @status))
                  AND (@minPrice IS NULL OR u.[base_price] >= @minPrice)
                  AND (@maxPrice IS NULL OR u.[base_price] <= @maxPrice)
                  AND (@minBedrooms IS NULL OR ut.[bedrooms] >= @minBedrooms)
                  AND (@hasSearch = 0 OR (u.[unit_code] LIKE @searchPattern OR (u.[street_number] IS NOT NULL AND u.[street_number] LIKE @searchPattern)));";
            countCmd.Parameters.Add(new SqlParameter("@phaseId", SqlDbType.Int) { Value = (object?)request.PhaseId ?? DBNull.Value });
            countCmd.Parameters.Add(new SqlParameter("@projectId", SqlDbType.Int) { Value = (object?)request.ProjectId ?? DBNull.Value });
            countCmd.Parameters.Add(new SqlParameter("@unitTypeId", SqlDbType.Int) { Value = (object?)request.UnitTypeId ?? DBNull.Value });
            countCmd.Parameters.Add(new SqlParameter("@status", SqlDbType.NVarChar, 50) { Value = (object?)request.Status ?? DBNull.Value });
            countCmd.Parameters.Add(new SqlParameter("@isAvailableSearch", SqlDbType.Bit) { Value = isAvailableSearch });
            countCmd.Parameters.Add(new SqlParameter("@utcNow", SqlDbType.DateTime2) { Value = utcNow });
            countCmd.Parameters.Add(new SqlParameter("@minPrice", SqlDbType.Decimal) { Value = (object?)request.MinPrice ?? DBNull.Value });
            countCmd.Parameters.Add(new SqlParameter("@maxPrice", SqlDbType.Decimal) { Value = (object?)request.MaxPrice ?? DBNull.Value });
            countCmd.Parameters.Add(new SqlParameter("@minBedrooms", SqlDbType.Int) { Value = (object?)request.MinBedrooms ?? DBNull.Value });
            countCmd.Parameters.Add(new SqlParameter("@hasSearch", SqlDbType.Bit) { Value = hasSearch });
            countCmd.Parameters.Add(new SqlParameter("@searchPattern", SqlDbType.NVarChar, 100) { Value = (object?)searchPattern ?? DBNull.Value });
            var scalar = await countCmd.ExecuteScalarAsync(cancellationToken);
            totalCount = Convert.ToInt32(scalar);
        }

        return new PaginatedList<UnitDto>(items, totalCount, pageNumber, pageSize);
    }
}
