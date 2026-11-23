using System.Data.Common;
using EstatePulse.Modules.Units.DTOs;

namespace EstatePulse.Modules.Units.Queries;

internal static class UnitTypeQueryMapping
{
    public static UnitTypeDto ReadUnitTypeDto(DbDataReader reader, int startIndex = 0)
    {
        return new UnitTypeDto
        {
            Id = reader.GetInt32(startIndex + 0),
            TypeName = reader.GetString(startIndex + 1),
            Code = reader.GetString(startIndex + 2),
            Bedrooms = reader.GetInt32(startIndex + 3),
            Bathrooms = reader.GetInt32(startIndex + 4),
            ParkingSlots = reader.GetInt32(startIndex + 5),
            Floors = reader.GetInt32(startIndex + 6),
            BuildingSize = reader.GetDecimal(startIndex + 7),
            LandSizeRecommended = reader.IsDBNull(startIndex + 8) ? null : reader.GetDecimal(startIndex + 8),
            FloorPlanImageUrl = reader.IsDBNull(startIndex + 9) ? null : reader.GetString(startIndex + 9),
            SpecificationJson = reader.IsDBNull(startIndex + 10) ? null : reader.GetString(startIndex + 10),
            TotalUnitsCount = reader.IsDBNull(startIndex + 11) ? 0 : reader.GetInt32(startIndex + 11)
        };
    }
}
