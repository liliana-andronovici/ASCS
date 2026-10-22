using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class TwoFactorAuthEntityTypeConfiguration
    : IEntityTypeConfiguration<TwoFactorAuthEntity>
{
    public void Configure(
        EntityTypeBuilder<TwoFactorAuthEntity> builder)
    {
        builder.ToTable("TwoFactorAuths");

        builder.HasKey(twoFactor => twoFactor.Id);

        builder.HasIndex(twoFactor => twoFactor.EmployeeId)
            .IsUnique();

        builder.Property(twoFactor => twoFactor.ProtectedSecret)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(twoFactor => twoFactor.RecoveryCodeHashes)
            .IsRequired()
            .HasColumnType("text[]");

        builder.Property(twoFactor => twoFactor.ChallengeTokenHash)
            .HasMaxLength(64);

        builder.HasIndex(twoFactor => twoFactor.ChallengeTokenHash)
            .IsUnique();

        builder.HasOne(twoFactor => twoFactor.Employee)
            .WithOne(employee => employee.TwoFactorAuth)
            .HasForeignKey<TwoFactorAuthEntity>(
                twoFactor => twoFactor.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}