using EstatePulse.Models;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                // Unique constraint on Email
                entity.HasIndex(u => u.Email)
                      .IsUnique();

                // Store UserRole enum as a string in the database
                entity.Property(u => u.Role)
                      .HasConversion<string>();

                // Default values
                entity.Property(u => u.IsActive)
                      .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}