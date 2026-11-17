using EstatePulse.Models;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserFollows> UserFollows => Set<UserFollows>();
    public DbSet<Agent> Agents => Set<Agent>();
    public DbSet<AgentLead> AgentLeads => Set<AgentLead>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectPhase> ProjectPhases => Set<ProjectPhase>();
    public DbSet<UnitType> UnitTypes => Set<UnitType>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<UnitMedia> UnitMedia => Set<UnitMedia>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Commission> Commissions => Set<Commission>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<InstallmentSchedule> InstallmentSchedules => Set<InstallmentSchedule>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<CustomerKYCDocument> CustomerKYCDocuments => Set<CustomerKYCDocument>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Users
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.CreatedAt);
            entity.HasIndex(u => u.FullName);
            entity.Property(u => u.Role).HasConversion<string>();
            entity.Property(u => u.IsActive).HasDefaultValue(true);
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // 2. UserFollows
        modelBuilder.Entity<UserFollows>(entity =>
        {
            entity.HasIndex(uf => new { uf.FollowingUserId, uf.FollowedUserId }).IsUnique();

            entity.HasOne(uf => uf.FollowingUser)
                  .WithMany(u => u.Followings)
                  .HasForeignKey(uf => uf.FollowingUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(uf => uf.FollowedUser)
                  .WithMany(u => u.Followers)
                  .HasForeignKey(uf => uf.FollowedUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 3. Agents
        modelBuilder.Entity<Agent>(entity =>
        {
            entity.HasIndex(a => a.UserId).IsUnique();
            entity.HasIndex(a => a.AgentCode).IsUnique();

            entity.HasOne(a => a.User)
                  .WithOne(u => u.AgentProfile)
                  .HasForeignKey<Agent>(a => a.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 4. AgentLeads
        modelBuilder.Entity<AgentLead>(entity =>
        {
            entity.HasOne(al => al.Agent)
                  .WithMany(a => a.Leads)
                  .HasForeignKey(al => al.AgentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(al => al.Customer)
                  .WithMany()
                  .HasForeignKey(al => al.CustomerId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 5. Projects
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasIndex(p => p.Slug).IsUnique();
        });

        // 6. ProjectPhases
        modelBuilder.Entity<ProjectPhase>(entity =>
        {
            entity.HasIndex(pp => new { pp.ProjectId, pp.Slug }).IsUnique();

            entity.HasOne(pp => pp.Project)
                  .WithMany(p => p.Phases)
                  .HasForeignKey(pp => pp.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 7. UnitTypes
        modelBuilder.Entity<UnitType>(entity =>
        {
            entity.HasIndex(ut => ut.Code).IsUnique();
        });

        // 8. Units
        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasIndex(u => new { u.PhaseId, u.UnitCode }).IsUnique();

            entity.HasOne(u => u.Phase)
                  .WithMany(p => p.Units)
                  .HasForeignKey(u => u.PhaseId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(u => u.UnitType)
                  .WithMany(ut => ut.Units)
                  .HasForeignKey(u => u.UnitTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(u => u.CurrentHoldUser)
                  .WithMany()
                  .HasForeignKey(u => u.CurrentHoldUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 9. UnitMedia
        modelBuilder.Entity<UnitMedia>(entity =>
        {
            entity.HasOne(um => um.Unit)
                  .WithMany(u => u.Media)
                  .HasForeignKey(um => um.UnitId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(um => um.UnitType)
                  .WithMany(ut => ut.Media)
                  .HasForeignKey(um => um.UnitTypeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 10. Bookings
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(b => b.BookingReference).IsUnique();

            entity.HasOne(b => b.Unit)
                  .WithMany(u => u.Bookings)
                  .HasForeignKey(b => b.UnitId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.Customer)
                  .WithMany(u => u.Bookings)
                  .HasForeignKey(b => b.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.Agent)
                  .WithMany(a => a.Bookings)
                  .HasForeignKey(b => b.AgentId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 11. Commissions
        modelBuilder.Entity<Commission>(entity =>
        {
            entity.HasOne(c => c.Booking)
                  .WithMany(b => b.Commissions)
                  .HasForeignKey(c => c.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Agent)
                  .WithMany(a => a.Commissions)
                  .HasForeignKey(c => c.AgentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.ApprovedByUser)
                  .WithMany()
                  .HasForeignKey(c => c.ApprovedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 12. Payments
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(p => p.PaymentReference).IsUnique();

            entity.HasOne(p => p.Booking)
                  .WithMany(b => b.Payments)
                  .HasForeignKey(p => p.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Customer)
                  .WithMany(u => u.Payments)
                  .HasForeignKey(p => p.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.VerifiedByUser)
                  .WithMany()
                  .HasForeignKey(p => p.VerifiedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 13. InstallmentSchedules
        modelBuilder.Entity<InstallmentSchedule>(entity =>
        {
            entity.HasOne(s => s.Booking)
                  .WithMany(b => b.InstallmentSchedules)
                  .HasForeignKey(s => s.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Payment)
                  .WithMany(p => p.InstallmentSchedules)
                  .HasForeignKey(s => s.PaymentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 14. Contracts
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasIndex(c => c.ContractNumber).IsUnique();

            entity.HasOne(c => c.Booking)
                  .WithMany(b => b.Contracts)
                  .HasForeignKey(c => c.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 15. CustomerKYCDocuments
        modelBuilder.Entity<CustomerKYCDocument>(entity =>
        {
            entity.HasOne(k => k.Customer)
                  .WithMany(u => u.KycDocuments)
                  .HasForeignKey(k => k.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(k => k.VerifiedByUser)
                  .WithMany()
                  .HasForeignKey(k => k.VerifiedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 16. AuditLogs
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasOne(a => a.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(a => a.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
