using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class OnboardingEntityTypeConfiguration : IEntityTypeConfiguration<OnboardingEntity>
{
    public void Configure(EntityTypeBuilder<OnboardingEntity> builder)
    {
        builder.ToTable("Onboardings");

        builder.HasOne(x => x.Employee)
            .WithOne()
            .HasForeignKey<OnboardingEntity>(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Buddy)
            .WithMany()
            .HasForeignKey(x => x.BuddyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}