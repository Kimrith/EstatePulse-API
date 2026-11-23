using System.Data.Common;
using EstatePulse.Common.Enums;
using EstatePulse.Modules.Users.DTOs;

namespace EstatePulse.Modules.Users.Queries;

internal static class UserQueryMapping
{
    public static UserDto ReadUserDto(DbDataReader reader, int startIndex = 0)
    {
        var roleStr = reader.IsDBNull(startIndex + 4) ? null : reader.GetString(startIndex + 4);
        Enum.TryParse<UserRole>(roleStr, true, out var role);

        return new UserDto
        {
            Id = reader.GetInt32(startIndex + 0),
            FullName = reader.GetString(startIndex + 1),
            Email = reader.GetString(startIndex + 2),
            PhoneNumber = reader.GetString(startIndex + 3),
            Role = role,
            NationalIdCard = reader.IsDBNull(startIndex + 5) ? null : reader.GetString(startIndex + 5),
            AvatarUrl = reader.IsDBNull(startIndex + 6) ? null : reader.GetString(startIndex + 6),
            IsActive = reader.GetBoolean(startIndex + 7),
            CreatedAt = reader.GetDateTime(startIndex + 8),
            UpdatedAt = reader.IsDBNull(startIndex + 9) ? null : reader.GetDateTime(startIndex + 9)
        };
    }
}
