using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class RoomEntity : BaseEntity
{
    public string Name { get; set; }
    public int Floor { get; set; }
    public int Seats { get; set; }
    public string? Amenities { get; set; }

    public int LocationId { get; set; }
    public virtual LocationEntity Location { get; set; }

    public virtual ICollection<RoomBookingEntity> RoomBookings { get; set; }
        = new List<RoomBookingEntity>();
}