using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class RoleEntityTypeConfiguration
    : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(role => role.Name)
            .IsUnique();

        builder.HasMany(role => role.Permissions)
            .WithMany()
            .UsingEntity<RolePermissionEntity>(
                right => right
                    .HasOne(join => join.Permission)
                    .WithMany()
                    .HasForeignKey(join => join.PermissionId),
                left => left
                    .HasOne(join => join.Role)
                    .WithMany()
                    .HasForeignKey(join => join.RoleId),
                join =>
                {
                    join.ToTable("RolePermissions");
                    join.HasKey(item => new
                    {
                        item.RoleId,
                        item.PermissionId
                    });
                });

        builder.HasMany(role => role.Employees)
            .WithMany(employee => employee.Roles)
            .UsingEntity<EmployeeRoleEntity>(
                right => right
                    .HasOne(join => join.Employee)
                    .WithMany()
                    .HasForeignKey(join => join.EmployeeId),
                left => left
                    .HasOne(join => join.Role)
                    .WithMany()
                    .HasForeignKey(join => join.RoleId),
                join =>
                {
                    join.ToTable("EmployeeRoles");
                    join.HasKey(item => new
                    {
                        item.EmployeeId,
                        item.RoleId
                    });
                });
    }
}