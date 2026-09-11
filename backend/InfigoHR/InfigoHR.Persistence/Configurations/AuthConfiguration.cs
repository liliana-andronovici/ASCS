using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class AuthConfiguration : IEntityTypeConfiguration<Auth>
{
    public void Configure(EntityTypeBuilder<Auth> builder)
    {
        builder.ToTable("Auth");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.ResetToken).HasMaxLength(250);
        builder.HasOne(x => x.Employee)
            .WithOne(x => x.Auth)
            .HasForeignKey<Auth>(x => x.EmployeeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
