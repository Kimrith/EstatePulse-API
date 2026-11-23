using EstatePulse.Modules.Bookings.Models;
using EstatePulse.Modules.Agents.Models;
using EstatePulse.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Bookings.Persistence;

public class CommissionConfiguration : IEntityTypeConfiguration<Commission>
{
    public void Configure(EntityTypeBuilder<Commission> builder)
    {
        builder.HasOne<Booking>()
              .WithMany()
              .HasForeignKey(c => c.BookingId)
              .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Agent>()
              .WithMany()
              .HasForeignKey(c => c.AgentId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(c => c.ApprovedByUserId)
              .OnDelete(DeleteBehavior.SetNull);
    }
}
