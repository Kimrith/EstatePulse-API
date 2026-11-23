using System.Data.Common;
using EstatePulse.Modules.Units.DTOs;

namespace EstatePulse.Modules.Units.Queries;

internal static class UnitMediaQueryMapping
{
    public static UnitMediaDto ReadUnitMediaDto(DbDataReader reader)
    {
        return new UnitMediaDto
        {
            Id = reader.GetInt32(0),
            UnitId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
            UnitCode = reader.IsDBNull(2) ? null : reader.GetString(2),
            UnitTypeId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
            UnitTypeCode = reader.IsDBNull(4) ? null : reader.GetString(4),
            MediaType = reader.GetString(5),
            MediaUrl = reader.GetString(6),
            SortOrder = reader.GetInt32(7),
            IsFeatured = reader.GetBoolean(8)
        };
    }
}
