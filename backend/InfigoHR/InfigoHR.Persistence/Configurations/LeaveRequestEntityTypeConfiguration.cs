using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class LeaveRequestEntityTypeConfiguration : IEntityTypeConfiguration<LeaveRequestEntity>
{
    public void Configure(EntityTypeBuilder<LeaveRequestEntity> builder)
    {
        builder.ToTable("LeaveRequests");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.StartDateUtc)
            .IsRequired();

        builder.Property(l => l.EndDateUtc)
            .IsRequired();

        builder.Property(l => l.Note)
            .HasMaxLength(500);

        builder.Property(l => l.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(l => l.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(l => l.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
