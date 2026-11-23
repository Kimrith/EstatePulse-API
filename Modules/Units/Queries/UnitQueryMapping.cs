using System.Data.Common;
using EstatePulse.Modules.Units.DTOs;

namespace EstatePulse.Modules.Units.Queries;

internal static class UnitQueryMapping
{
    public static UnitDto ReadUnitDto(DbDataReader reader, DateTime utcNow, int startIndex = 0)
    {
        var id = reader.GetInt32(startIndex + 0);
        var phaseId = reader.GetInt32(startIndex + 1);
        var phaseName = reader.IsDBNull(startIndex + 2) ? null : reader.GetString(startIndex + 2);
        var projectId = reader.IsDBNull(startIndex + 3) ? (int?)null : reader.GetInt32(startIndex + 3);
        var projectName = reader.IsDBNull(startIndex + 4) ? null : reader.GetString(startIndex + 4);
        var unitTypeId = reader.GetInt32(startIndex + 5);
        var unitTypeCode = reader.IsDBNull(startIndex + 6) ? null : reader.GetString(startIndex + 6);
        var unitTypeName = reader.IsDBNull(startIndex + 7) ? null : reader.GetString(startIndex + 7);
        var unitCode = reader.GetString(startIndex + 8);
        var streetNumber = reader.IsDBNull(startIndex + 9) ? null : reader.GetString(startIndex + 9);
        var landAreaSqm = reader.GetDecimal(startIndex + 10);
        var builtAreaSqm = reader.GetDecimal(startIndex + 11);
        var basePrice = reader.GetDecimal(startIndex + 12);
        var status = reader.GetString(startIndex + 13);
        DateTime? holdExpiresAt = reader.IsDBNull(startIndex + 14) ? null : reader.GetDateTime(startIndex + 14);
        int? currentHoldUserId = reader.IsDBNull(startIndex + 15) ? null : reader.GetInt32(startIndex + 15);
        var currentHoldUserName = reader.IsDBNull(startIndex + 16) ? null : reader.GetString(startIndex + 16);
        var svgElementId = reader.IsDBNull(startIndex + 17) ? null : reader.GetString(startIndex + 17);

        byte[]? rowVersionBytes = null;
        if (!reader.IsDBNull(startIndex + 18))
        {
            rowVersionBytes = (byte[])reader.GetValue(startIndex + 18);
        }

        var createdAt = reader.GetDateTime(startIndex + 19);
        DateTime? updatedAt = reader.IsDBNull(startIndex + 20) ? null : reader.GetDateTime(startIndex + 20);

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
