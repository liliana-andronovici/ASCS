using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<EmployeeEntity> Employees => Set<EmployeeEntity>();
    public DbSet<DepartmentEntity> Departments => Set<DepartmentEntity>();
    public DbSet<LocationEntity> Locations => Set<LocationEntity>();
    public DbSet<LeaveRequestEntity> TimeOff => Set<LeaveRequestEntity>();
    public DbSet<AuthEntity> Auths => Set<AuthEntity>();
    public DbSet<TwoFactorAuthEntity> TwoFactorAuths => Set<TwoFactorAuthEntity>();
    public DbSet<TrustedDeviceEntity> TrustedDevices => Set<TrustedDeviceEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
