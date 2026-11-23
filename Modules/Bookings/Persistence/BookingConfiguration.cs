using EstatePulse.Modules.Bookings.Models;
using EstatePulse.Modules.Units.Models;
using EstatePulse.Modules.Users.Models;
using EstatePulse.Modules.Agents.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Bookings.Persistence;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasIndex(b => b.BookingReference).IsUnique();

        builder.HasOne<Unit>()
              .WithMany()
              .HasForeignKey(b => b.UnitId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(b => b.CustomerId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Agent>()
              .WithMany()
              .HasForeignKey(b => b.AgentId)
              .OnDelete(DeleteBehavior.SetNull);
    }
}
