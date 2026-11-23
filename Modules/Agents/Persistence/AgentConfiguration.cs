using EstatePulse.Modules.Agents.Models;
using EstatePulse.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Agents.Persistence;

public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.HasIndex(a => a.UserId).IsUnique();
        builder.HasIndex(a => a.AgentCode).IsUnique();

        builder.HasOne<User>()
              .WithOne()
              .HasForeignKey<Agent>(a => a.UserId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
