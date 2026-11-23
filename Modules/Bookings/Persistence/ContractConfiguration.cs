using EstatePulse.Modules.Bookings.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Bookings.Persistence;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasIndex(c => c.ContractNumber).IsUnique();

        builder.HasOne<Booking>()
              .WithMany()
              .HasForeignKey(c => c.BookingId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
