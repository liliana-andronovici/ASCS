using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class AuthConfiguration : IEntityTypeConfiguration<Auth>
{
    public void Configure(EntityTypeBuilder<Auth> builder)
    {
        // Denumire tabela
        builder.ToTable("Auth");

        // PK bazata pe ID-ul angajatului
        builder.HasKey(a => a.EmployeeId);

        // Validare camp parola
        builder.Property(a => a.PasswordHash)
            .IsRequired();

        builder.Property(a => a.ResetToken)
            .HasMaxLength(250);

        // Relatie 1:1 cu tabela Employees (foloseste navigatia Employee.Auth, fara coloana shadow)
        builder.HasOne(a => a.Employee)
            .WithOne(e => e.Auth)
            .HasForeignKey<Auth>(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
