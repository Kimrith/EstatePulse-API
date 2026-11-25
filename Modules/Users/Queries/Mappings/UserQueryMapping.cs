using System.Data.Common;
using EstatePulse.Common.Database;
using EstatePulse.Common.Enums;
using EstatePulse.Modules.Users.DTOs;

namespace EstatePulse.Modules.Users.Queries;

internal static class UserQueryMapping
{
    public static UserDto ReadUserDto(DbDataReader reader, int offset = 0)
    {
        var roleStr = reader.GetNullableString(offset + 4);
        Enum.TryParse<UserRole>(roleStr, true, out var role);

        return new UserDto
        {
            Id = reader.GetInt32(offset),
            FullName = reader.GetString(offset + 1),
            Email = reader.GetString(offset + 2),
            PhoneNumber = reader.GetString(offset + 3),
            Role = role,
            NationalIdCard = reader.GetNullableString(offset + 5),
            AvatarUrl = reader.GetNullableString(offset + 6),
            IsActive = reader.GetBoolean(offset + 7),
            CreatedAt = reader.GetDateTime(offset + 8),
            UpdatedAt = reader.GetNullableDateTime(offset + 9)
        };
    }
}
