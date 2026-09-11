using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class TrustedDeviceConfiguration : IEntityTypeConfiguration<TrustedDevice>
{
    public void Configure(EntityTypeBuilder<TrustedDevice> builder)
    {
        builder.ToTable("TrustedDevice");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DeviceToken).IsRequired().HasMaxLength(250);
        builder.HasIndex(x => x.DeviceToken).IsUnique();
        builder.Property(x => x.TrustedUntilUtc).IsRequired();
        builder.HasOne(x => x.Employee)
            .WithMany(x => x.TrustedDevices)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
