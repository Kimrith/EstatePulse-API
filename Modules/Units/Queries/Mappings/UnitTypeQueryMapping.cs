using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;

namespace EstatePulse.Modules.Units.Queries;

internal static class UnitTypeQueryMapping
{
    public static UnitTypeDto ReadUnitTypeDto(DbDataReader reader, int offset = 0)
    {
        return new UnitTypeDto
        {
            Id = reader.GetInt32(offset),
            TypeName = reader.GetString(offset + 1),
            Code = reader.GetString(offset + 2),
            Bedrooms = reader.GetInt32(offset + 3),
            Bathrooms = reader.GetInt32(offset + 4),
            ParkingSlots = reader.GetInt32(offset + 5),
            Floors = reader.GetInt32(offset + 6),
            BuildingSize = reader.GetDecimal(offset + 7),
            LandSizeRecommended = reader.GetNullableDecimal(offset + 8),
            FloorPlanImageUrl = reader.GetNullableString(offset + 9),
            SpecificationJson = reader.GetNullableString(offset + 10),
            TotalUnitsCount = reader.GetNullableInt32(offset + 11) ?? 0
        };
    }
}
