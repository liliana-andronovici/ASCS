using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class EquipmentTypeEntityTypeConfiguration : IEntityTypeConfiguration<EquipmentTypeEntity>
{
    public void Configure(EntityTypeBuilder<EquipmentTypeEntity> builder)
    {
        builder.ToTable("EquipmentType");
        builder.HasKey(type => type.Id);
        builder.Property(type => type.Name).IsRequired().HasMaxLength(80);
    }
}
