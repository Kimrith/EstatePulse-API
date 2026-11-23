using EstatePulse.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Users.Persistence;

public class CustomerKYCDocumentConfiguration : IEntityTypeConfiguration<CustomerKYCDocument>
{
    public void Configure(EntityTypeBuilder<CustomerKYCDocument> builder)
    {
        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(k => k.CustomerId)
              .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(k => k.VerifiedByUserId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
