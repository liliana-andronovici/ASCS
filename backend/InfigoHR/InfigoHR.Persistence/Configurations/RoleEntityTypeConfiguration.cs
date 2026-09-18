using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public sealed class RoleEntityTypeConfiguration
    : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(role => role.Id);
        builder.Property(role => role.Id)
            .ValueGeneratedNever();

        builder.Property(role => role.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(role => role.Name)
            .IsUnique();

        builder.HasMany(role => role.Permissions)
            .WithMany()
            .UsingEntity(
                "RolePermissions",
                right => right.HasOne(typeof(PermissionEntity))
                    .WithMany()
                    .HasForeignKey("PermissionId"),
                left => left.HasOne(typeof(RoleEntity))
                    .WithMany()
                    .HasForeignKey("RoleId"),
                join => join.HasKey("RoleId", "PermissionId"));

        builder.HasMany(role => role.Employees)
            .WithMany(employee => employee.Roles)
            .UsingEntity(
                "EmployeeRoles",
                right => right.HasOne(typeof(EmployeeEntity))
                    .WithMany()
                    .HasForeignKey("EmployeeId"),
                left => left.HasOne(typeof(RoleEntity))
                    .WithMany()
                    .HasForeignKey("RoleId"),
                join => join.HasKey("EmployeeId", "RoleId"));
    }
}