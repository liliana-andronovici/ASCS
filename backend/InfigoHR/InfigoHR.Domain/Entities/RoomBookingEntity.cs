using InfigoHR.Domain.Common;

namespace InfigoHR.Domain.Entities;

public class RoomBookingEntity : BaseEntity
{
    public int RoomId { get; set; }
    public virtual RoomEntity Room { get; set; }

    public int OrganizerEmployeeId { get; set; }
    public virtual EmployeeEntity OrganizerEmployee { get; set; }

    public string Title { get; set; }
    public DateTime StartDateTimeUtc { get; set; }
    public int DurationMinutes { get; set; }

    public virtual ICollection<EmployeeEntity> Employees { get; set; }
        = new List<EmployeeEntity>();
}