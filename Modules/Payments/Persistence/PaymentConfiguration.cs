using EstatePulse.Modules.Payments.Models;
using EstatePulse.Modules.Bookings.Models;
using EstatePulse.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Payments.Persistence;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasIndex(p => p.PaymentReference).IsUnique();

        builder.HasOne<Booking>()
              .WithMany()
              .HasForeignKey(p => p.BookingId)
              .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(p => p.CustomerId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(p => p.VerifiedByUserId)
              .OnDelete(DeleteBehavior.SetNull);
    }
}
