using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Denumire tabela
        builder.ToTable("Employees");

        // PK
        builder.HasKey(e => e.EmployeeId);

        // Validari si limite campuri text
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(150);

        // Email este unic in sistem (folosit si la login)
        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Started)
            .IsRequired();

        builder.Property(e => e.InviteToken)
            .HasMaxLength(250);

        // Conversie Enum in int pentru eficienta in DB
        builder.Property(e => e.Status)
            .HasConversion<int>()
            .IsRequired();

        // Relații si constrangeri Foreign Key (1:N)
        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Location)
            .WithMany(l => l.Employees)
            .HasForeignKey(e => e.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Auto-relatie: managerul unui angajat este tot un Employee.
        // Restrict pentru a evita ciclurile de cascade delete pe self-reference.
        builder.HasOne(e => e.Manager)
            .WithMany(e => e.DirectReports)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
