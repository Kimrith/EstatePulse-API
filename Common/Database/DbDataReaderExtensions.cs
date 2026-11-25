using System.Data.Common;

namespace EstatePulse.Common.Database;

public static class DbDataReaderExtensions
{
    public static int? GetNullableInt32(this DbDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);

    public static decimal? GetNullableDecimal(this DbDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? null : reader.GetDecimal(ordinal);

    public static string? GetNullableString(this DbDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);

    public static DateTime? GetNullableDateTime(this DbDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);

    public static bool? GetNullableBoolean(this DbDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? null : reader.GetBoolean(ordinal);

    public static Guid? GetNullableGuid(this DbDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? null : reader.GetGuid(ordinal);
}
