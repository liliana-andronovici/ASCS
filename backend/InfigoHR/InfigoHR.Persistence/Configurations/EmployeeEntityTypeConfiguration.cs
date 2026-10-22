using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class EmployeeEntityTypeConfiguration : IEntityTypeConfiguration<EmployeeEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        builder.Property(x => x.StartedOnUtc).IsRequired();
        builder.Property(x => x.InviteToken).HasMaxLength(250);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(40);
        builder.Property(x => x.Desk).HasMaxLength(150);
        builder.Property(x => x.EmployeeCode).HasMaxLength(50);
        builder.Property(x => x.EmploymentType).HasMaxLength(50);
        builder.Property(x => x.ContractType).HasMaxLength(100);
        builder.Property(x => x.Languages).IsRequired();
        builder.Property(x => x.WeeklyHours).HasPrecision(5, 2);
        builder.Property(x => x.ProfileVersion).IsConcurrencyToken();
        builder.HasIndex(x => x.ManagerId);
        builder.ToTable("Employees", table => table.HasCheckConstraint("CK_Employees_Manager_NotSelf", "\"ManagerId\" IS NULL OR \"ManagerId\" <> \"Id\""));

        builder.HasOne(x => x.Department)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Location)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Manager)
            .WithMany()
            .HasForeignKey(x => x.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
