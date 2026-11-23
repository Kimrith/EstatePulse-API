using EstatePulse.Modules.Units.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Units.Persistence;

public class UnitMediaConfiguration : IEntityTypeConfiguration<UnitMedia>
{
    public void Configure(EntityTypeBuilder<UnitMedia> builder)
    {
        builder.HasOne<Unit>()
              .WithMany()
              .HasForeignKey(um => um.UnitId)
              .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<UnitType>()
              .WithMany()
              .HasForeignKey(um => um.UnitTypeId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
