using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public sealed class PermissionEntityTypeConfiguration
    : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(
        EntityTypeBuilder<PermissionEntity> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(permission => permission.Id);
        builder.Property(permission => permission.Id)
            .ValueGeneratedNever();

        builder.Property(permission => permission.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(permission => permission.Name)
            .IsUnique();
    }
}