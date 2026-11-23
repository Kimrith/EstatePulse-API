using EstatePulse.Modules.AuditLogs.Models;
using EstatePulse.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.AuditLogs.Persistence;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(a => a.UserId)
              .OnDelete(DeleteBehavior.SetNull);
    }
}
