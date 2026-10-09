using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfigoHR.Persistence.Configurations;

public class RoomBookingEntityTypeConfiguration
    : IEntityTypeConfiguration<RoomBookingEntity>
{
    public void Configure(EntityTypeBuilder<RoomBookingEntity> builder)
    {
        builder.ToTable("RoomBookings");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.StartDateTimeUtc)
            .IsRequired();

        builder.HasOne(x => x.Room)
            .WithMany(x => x.RoomBookings)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OrganizerEmployee)
            .WithMany()
            .HasForeignKey(x => x.OrganizerEmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Employees)
            .WithMany(x => x.RoomBookings)
            .UsingEntity<EmployeeRoomBookingEntity>(
                right => right
                    .HasOne(x => x.Employee)
                    .WithMany()
                    .HasForeignKey(x => x.EmployeeId),
                left => left
                    .HasOne(x => x.RoomBooking)
                    .WithMany()
                    .HasForeignKey(x => x.RoomBookingId),
                join =>
                {
                    join.ToTable("EmployeeRoomBookings");

                    join.HasKey(x => new
                    {
                        x.EmployeeId,
                        x.RoomBookingId
                    });
                });
    }
}