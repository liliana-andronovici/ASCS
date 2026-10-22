using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<EmployeeEntity> Employees => Set<EmployeeEntity>();
    public DbSet<EmployeeDocumentEntity> EmployeeDocuments => Set<EmployeeDocumentEntity>();
    public DbSet<EmployeeHistoryEntity> EmployeeHistory => Set<EmployeeHistoryEntity>();
    public DbSet<EquipmentEntity> Equipment => Set<EquipmentEntity>();
    public DbSet<EquipmentTypeEntity> EquipmentTypes => Set<EquipmentTypeEntity>();
    public DbSet<EmployeeEquipmentEntity> EmployeeEquipment => Set<EmployeeEquipmentEntity>();
    public DbSet<DepartmentEntity> Departments => Set<DepartmentEntity>();
    public DbSet<LocationEntity> Locations => Set<LocationEntity>();
    public DbSet<LeaveRequestEntity> LeaveRequests => Set<LeaveRequestEntity>();
    public DbSet<AuthEntity> Auths => Set<AuthEntity>();
    public DbSet<TwoFactorAuthEntity> TwoFactorAuths => Set<TwoFactorAuthEntity>();
    public DbSet<TrustedDeviceEntity> TrustedDevices => Set<TrustedDeviceEntity>();
    public DbSet<OnboardingEntity> Onboardings => Set<OnboardingEntity>();
    public DbSet<OnboardingTaskEntity> OnboardingTasks => Set<OnboardingTaskEntity>();
    public DbSet<RoomEntity> Rooms => Set<RoomEntity>();
    public DbSet<RoomBookingEntity> RoomBookings => Set<RoomBookingEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
