using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class TwoFactorAuthConfiguration : IEntityTypeConfiguration<TwoFactorAuth>
{
    public void Configure(EntityTypeBuilder<TwoFactorAuth> builder)
    {
        builder.ToTable("TwoFactorAuth");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.Property(x => x.SecretKey).IsRequired().HasMaxLength(100);
        builder.HasOne(x => x.Employee)
            .WithOne(x => x.TwoFactorAuth)
            .HasForeignKey<TwoFactorAuth>(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
