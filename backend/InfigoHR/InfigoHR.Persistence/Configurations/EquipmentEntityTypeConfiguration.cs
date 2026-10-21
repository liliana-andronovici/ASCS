using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class EquipmentEntityTypeConfiguration : IEntityTypeConfiguration<EquipmentEntity>
{
    public void Configure(EntityTypeBuilder<EquipmentEntity> builder)
    {
        builder.ToTable("Equipment");
        builder.HasKey(equipment => equipment.Id);
        builder.Property(equipment => equipment.Name).IsRequired().HasMaxLength(150);
        builder.Property(equipment => equipment.Code).IsRequired().HasMaxLength(80);
        builder.HasOne(equipment => equipment.EquipmentType)
            .WithMany()
            .HasForeignKey(equipment => equipment.EquipmentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
