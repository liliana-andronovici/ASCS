using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class TwoFactorAuthConfiguration : IEntityTypeConfiguration<TwoFactorAuth>
{
    public void Configure(EntityTypeBuilder<TwoFactorAuth> builder)
    {
        // Denumire tabela
        builder.ToTable("TwoFactorAuth");

        // PK bazata pe ID-ul angajatului
        builder.HasKey(t => t.EmployeeId);

        // Validare camp cheie secreta 2FA
        builder.Property(t => t.SecretKey)
            .IsRequired()
            .HasMaxLength(100);

        // Relatie speciala 1:1 cu tabela Employee
        builder.HasOne(t => t.Employee)
            .WithOne(e => e.TwoFactorAuth)
            .HasForeignKey<TwoFactorAuth>(t => t.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}