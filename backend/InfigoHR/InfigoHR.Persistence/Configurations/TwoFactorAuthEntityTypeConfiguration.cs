using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class TwoFactorAuthEntityTypeConfiguration : IEntityTypeConfiguration<TwoFactorAuthEntity>
{
    public void Configure(EntityTypeBuilder<TwoFactorAuthEntity> builder)
    {
        builder.ToTable("TwoFactorAuths");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.Property(x => x.SecretKey).IsRequired().HasMaxLength(100);
        builder.HasOne(x => x.Employee)
            .WithOne(x => x.TwoFactorAuth)
            .HasForeignKey<TwoFactorAuthEntity>(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
