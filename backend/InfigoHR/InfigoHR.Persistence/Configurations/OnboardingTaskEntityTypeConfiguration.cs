using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class OnboardingTaskEntityTypeConfiguration : IEntityTypeConfiguration<OnboardingTaskEntity>
{
    public void Configure(EntityTypeBuilder<OnboardingTaskEntity> builder)
    {
        builder.ToTable("OnboardingTasks");

        builder.HasOne(x => x.Onboarding)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.OnboardingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}