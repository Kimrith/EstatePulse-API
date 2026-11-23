using EstatePulse.Modules.Payments.Models;
using EstatePulse.Modules.Bookings.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Payments.Persistence;

public class InstallmentScheduleConfiguration : IEntityTypeConfiguration<InstallmentSchedule>
{
    public void Configure(EntityTypeBuilder<InstallmentSchedule> builder)
    {
        builder.HasIndex(s => new { s.BookingId, s.InstallmentNumber }).IsUnique();

        builder.HasOne<Booking>()
              .WithMany()
              .HasForeignKey(s => s.BookingId)
              .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Payment>()
              .WithMany()
              .HasForeignKey(s => s.PaymentId)
              .OnDelete(DeleteBehavior.SetNull);
    }
}
