using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class TrustedDeviceConfiguration : IEntityTypeConfiguration<TrustedDevice>
{
    public void Configure(EntityTypeBuilder<TrustedDevice> builder)
    {
        // Denumire tabela
        builder.ToTable("TrustedDevice");

        // PK bazata pe token-ul unic al browserului
        builder.HasKey(t => t.DeviceToken);

        builder.Property(t => t.DeviceToken)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(t => t.TrustedUntil)
            .IsRequired();

        // Relatie Foreign Key (1:N) cu tabela Employees
        builder.HasOne(t => t.Employee)
            .WithMany(e => e.TrustedDevices)
            .HasForeignKey(t => t.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}