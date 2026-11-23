using EstatePulse.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Users.Persistence;

public class UserFollowsConfiguration : IEntityTypeConfiguration<UserFollows>
{
    public void Configure(EntityTypeBuilder<UserFollows> builder)
    {
        builder.HasIndex(uf => new { uf.FollowingUserId, uf.FollowedUserId }).IsUnique();

        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(uf => uf.FollowingUserId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(uf => uf.FollowedUserId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
