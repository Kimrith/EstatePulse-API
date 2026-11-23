using EstatePulse.Modules.Projects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EstatePulse.Modules.Projects.Persistence;

public class ProjectPhaseConfiguration : IEntityTypeConfiguration<ProjectPhase>
{
    public void Configure(EntityTypeBuilder<ProjectPhase> builder)
    {
        builder.HasIndex(pp => new { pp.ProjectId, pp.Slug }).IsUnique();

        builder.HasOne<Project>()
              .WithMany()
              .HasForeignKey(pp => pp.ProjectId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
