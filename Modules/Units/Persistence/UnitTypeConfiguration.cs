using EstatePulse.Modules.Units.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Units.Persistence;

public class UnitTypeConfiguration : IEntityTypeConfiguration<UnitType>
{
    public void Configure(EntityTypeBuilder<UnitType> builder)
    {
        builder.HasIndex(ut => ut.Code).IsUnique();
    }
}
