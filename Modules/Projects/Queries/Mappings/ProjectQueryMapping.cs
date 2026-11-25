using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Modules.Projects.DTOs;

namespace EstatePulse.Modules.Projects.Queries;

internal static class ProjectQueryMapping
{
    public static ProjectDto ReadProjectDto(DbDataReader reader, int offset = 0)
    {
        return new ProjectDto
        {
            Id = reader.GetInt32(offset),
            Name = reader.GetString(offset + 1),
            Slug = reader.GetString(offset + 2),
            LocationAddress = reader.GetNullableString(offset + 3),
            City = reader.GetNullableString(offset + 4),
            Latitude = reader.GetNullableDecimal(offset + 5),
            Longitude = reader.GetNullableDecimal(offset + 6),
            ThumbnailUrl = reader.GetNullableString(offset + 7),
            Description = reader.GetNullableString(offset + 8),
            IsPublished = reader.GetBoolean(offset + 9),
            CreatedAt = reader.GetDateTime(offset + 10)
        };
    }
}
