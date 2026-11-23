using EstatePulse.Modules.Units.Models;
using EstatePulse.Modules.Projects.Models;
using EstatePulse.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Units.Persistence;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.HasIndex(u => new { u.PhaseId, u.UnitCode }).IsUnique();

        builder.HasOne<ProjectPhase>()
              .WithMany()
              .HasForeignKey(u => u.PhaseId)
              .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<UnitType>()
              .WithMany()
              .HasForeignKey(u => u.UnitTypeId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(u => u.CurrentHoldUserId)
              .OnDelete(DeleteBehavior.SetNull);
    }
}
