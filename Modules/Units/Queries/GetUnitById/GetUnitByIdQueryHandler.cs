using System.Data;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Modules.Units.Queries;

public sealed class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery, UnitDto?>
{
    private readonly AppDbContext _context;

    public GetUnitByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<UnitDto?> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
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
            WHERE u.[id] = @id;";

        command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = request.Id });

        var utcNow = DateTime.UtcNow;
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return UnitQueryMapping.ReadUnitDto(reader, utcNow);
        }

        return null;
    }
}
