using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Projects.DTOs;

namespace EstatePulse.Modules.Projects.Queries;

internal static class ProjectPhaseQueryMapping
{
    public static ProjectPhaseDto ReadProjectPhaseDto(DbDataReader reader, int offset = 0)
    {
        return new ProjectPhaseDto
        {
            Id = reader.GetInt32(offset),
            ProjectId = reader.GetInt32(offset + 1),
            PhaseName = reader.GetString(offset + 2),
            Slug = reader.GetString(offset + 3),
            MasterPlanSvgUrl = reader.GetNullableString(offset + 4),
            LaunchDate = reader.GetNullableDateTime(offset + 5),
            ExpectedCompletionDate = reader.GetNullableDateTime(offset + 6),
            Status = reader.GetString(offset + 7),
            CreatedAt = reader.GetDateTime(offset + 8),
            ProjectName = reader.GetNullableString(offset + 9)
        };
    }
}
