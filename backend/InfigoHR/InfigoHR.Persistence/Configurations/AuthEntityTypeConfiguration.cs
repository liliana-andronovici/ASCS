using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class AuthEntityTypeConfiguration : IEntityTypeConfiguration<AuthEntity>
{
    public void Configure(EntityTypeBuilder<AuthEntity> builder)
    {
        builder.ToTable("Auths");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.ResetToken).HasMaxLength(250);
        builder.HasOne(x => x.Employee)
            .WithOne(x => x.Auth)
            .HasForeignKey<AuthEntity>(x => x.EmployeeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(auth => auth.ResetCodeHash).HasMaxLength(64);
        builder.Property(auth => auth.ConcurrencyVersion).IsConcurrencyToken();
    }
}
