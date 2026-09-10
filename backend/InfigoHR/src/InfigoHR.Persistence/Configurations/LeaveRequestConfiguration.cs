using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        // Denumire tabela
        builder.ToTable("LeaveRequest");

        // Cheie Primara (PK)
        builder.HasKey(l => l.Id);

        // Validari campuri data si text opțional
        builder.Property(l => l.StartDate)
            .IsRequired();

        builder.Property(l => l.EndDate)
            .IsRequired();

        builder.Property(l => l.Note)
            .HasMaxLength(500); // Limită rezonabilă pentru textul notei

        // Conversie Enum-uri în int pentru stocare eficientă
        builder.Property(l => l.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(l => l.StatusT)
            .HasConversion<int>()
            .IsRequired();

        // Relație Foreign Key (1:N) cu tabela Employees
        builder.HasOne(l => l.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade); // Dacă un angajat este șters, se șterg și cererile lui
    }
}