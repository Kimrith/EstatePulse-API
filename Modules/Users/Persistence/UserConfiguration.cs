using EstatePulse.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Users.Persistence;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.CreatedAt);
        builder.HasIndex(u => u.FullName);
        builder.Property(u => u.Role).HasConversion<string>();
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
