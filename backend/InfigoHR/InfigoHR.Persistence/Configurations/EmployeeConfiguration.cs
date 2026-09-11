using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
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
        builder.HasOne(x => x.Department)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.Location)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Manager)
            .WithMany()
            .HasForeignKey(x => x.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
