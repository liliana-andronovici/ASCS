using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class RoomEntityTypeConfiguration
    : IEntityTypeConfiguration<RoomEntity>
{
    public void Configure(EntityTypeBuilder<RoomEntity> builder)
    {
        builder.ToTable("Rooms");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Amenities)
            .HasMaxLength(500);

        builder.HasOne(x => x.Location)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}