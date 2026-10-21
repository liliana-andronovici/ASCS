using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class EmployeeEquipmentEntityTypeConfiguration : IEntityTypeConfiguration<EmployeeEquipmentEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeEquipmentEntity> builder)
    {
        builder.ToTable("EmployeeEquipment");
        builder.HasKey(equipment => equipment.Id);
        builder.Property(equipment => equipment.AssignedOnUtc).IsRequired();

        builder.HasOne<EmployeeEntity>()
            .WithMany()
            .HasForeignKey(equipment => equipment.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(equipment => equipment.Equipment)
            .WithMany()
            .HasForeignKey(equipment => equipment.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(equipment => equipment.EmployeeId)
            .HasDatabaseName("IX_EmployeeEquipment_EmployeeId");
        builder.HasIndex(equipment => equipment.EquipmentId)
            .IsUnique()
            .HasFilter("\"ReturnedOnUtc\" IS NULL")
            .HasDatabaseName("IX_EmployeeEquipment_ActiveAssignment");
    }
}
