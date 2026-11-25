using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;

namespace EstatePulse.Modules.Units.Queries;

internal static class UnitMediaQueryMapping
{
    public static UnitMediaDto ReadUnitMediaDto(DbDataReader reader, int offset = 0)
    {
        return new UnitMediaDto
        {
            Id = reader.GetInt32(offset),
            UnitId = reader.GetNullableInt32(offset + 1),
            UnitCode = reader.GetNullableString(offset + 2),
            UnitTypeId = reader.GetNullableInt32(offset + 3),
            UnitTypeCode = reader.GetNullableString(offset + 4),
            MediaType = reader.GetString(offset + 5),
            MediaUrl = reader.GetString(offset + 6),
            SortOrder = reader.GetInt32(offset + 7),
            IsFeatured = reader.GetBoolean(offset + 8)
        };
    }
}
