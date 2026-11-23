using EstatePulse.Modules.Agents.Models;
using EstatePulse.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Agents.Persistence;

public class AgentLeadConfiguration : IEntityTypeConfiguration<AgentLead>
{
    public void Configure(EntityTypeBuilder<AgentLead> builder)
    {
        builder.HasOne<Agent>()
              .WithMany()
              .HasForeignKey(al => al.AgentId)
              .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
              .WithMany()
              .HasForeignKey(al => al.CustomerId)
              .OnDelete(DeleteBehavior.SetNull);
    }
}
