using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Units.DTOs;

namespace EstatePulse.Modules.Units.Queries;

internal static class UnitQueryMapping
{
    public static UnitDto ReadUnitDto(DbDataReader reader, DateTime utcNow, int offset = 0)
    {
        var id = reader.GetInt32(offset);
        var phaseId = reader.GetInt32(offset + 1);
        var phaseName = reader.GetNullableString(offset + 2);
        var projectId = reader.GetNullableInt32(offset + 3);
        var projectName = reader.GetNullableString(offset + 4);
        var unitTypeId = reader.GetInt32(offset + 5);
        var unitTypeCode = reader.GetNullableString(offset + 6);
        var unitTypeName = reader.GetNullableString(offset + 7);
        var unitCode = reader.GetString(offset + 8);
        var streetNumber = reader.GetNullableString(offset + 9);
        var landAreaSqm = reader.GetDecimal(offset + 10);
        var builtAreaSqm = reader.GetDecimal(offset + 11);
        var basePrice = reader.GetDecimal(offset + 12);
        var status = reader.GetString(offset + 13);
        DateTime? holdExpiresAt = reader.GetNullableDateTime(offset + 14);
        int? currentHoldUserId = reader.GetNullableInt32(offset + 15);
        var currentHoldUserName = reader.GetNullableString(offset + 16);
        var svgElementId = reader.GetNullableString(offset + 17);

        byte[]? rowVersionBytes = null;
        if (!reader.IsDBNull(offset + 18))
        {
            rowVersionBytes = (byte[])reader.GetValue(offset + 18);
        }

        var createdAt = reader.GetDateTime(offset + 19);
        DateTime? updatedAt = reader.GetNullableDateTime(offset + 20);

        var isHoldActive = holdExpiresAt.HasValue && holdExpiresAt.Value > utcNow;
        var effectiveStatus = (!isHoldActive && status == "OnHold") ? "Available" : status;
        var holdRemainingSeconds = isHoldActive ? (int)(holdExpiresAt!.Value - utcNow).TotalSeconds : (int?)null;
        var rowVersionBase64 = rowVersionBytes != null ? Convert.ToBase64String(rowVersionBytes) : null;

        return new UnitDto
        {
            Id = id,
            PhaseId = phaseId,
            PhaseName = phaseName,
            ProjectId = projectId,
            ProjectName = projectName,
            UnitTypeId = unitTypeId,
            UnitTypeCode = unitTypeCode,
            UnitTypeName = unitTypeName,
            UnitCode = unitCode,
            StreetNumber = streetNumber,
            LandAreaSqm = landAreaSqm,
            BuiltAreaSqm = builtAreaSqm,
            BasePrice = basePrice,
            Status = effectiveStatus,
            HoldExpiresAt = isHoldActive ? holdExpiresAt : null,
            CurrentHoldUserId = isHoldActive ? currentHoldUserId : null,
            CurrentHoldUserName = isHoldActive ? currentHoldUserName : null,
            HoldRemainingSeconds = holdRemainingSeconds,
            SvgElementId = svgElementId,
            RowVersion = rowVersionBase64,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }
}
