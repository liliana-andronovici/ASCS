namespace InfigoHR.Domain.Entities;

public class EmployeeRoomBookingEntity
{
    public int EmployeeId { get; set; }
    public int RoomBookingId { get; set; }

    public virtual EmployeeEntity Employee { get; set; }
    public virtual RoomBookingEntity RoomBooking { get; set; }
}