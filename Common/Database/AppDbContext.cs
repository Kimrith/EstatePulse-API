using EstatePulse.Common.Enums;
using EstatePulse.Modules.Agents.Models;
using EstatePulse.Modules.AuditLogs.Models;
using EstatePulse.Modules.Bookings.Models;
using EstatePulse.Modules.Payments.Models;
using EstatePulse.Modules.Projects.Models;
using EstatePulse.Modules.Units.Models;
using EstatePulse.Modules.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace EstatePulse.Common.Database;

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

        // Apply entity configurations from all modules
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
