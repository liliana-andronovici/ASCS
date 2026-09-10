using InfigoHR.Domain.Enums;

namespace InfigoHR.Domain.Entities;

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; } // FK

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Note { get; set; } = string.Empty;


    public LeaveType Type { get; set; }
    public LeaveRequestStatus StatusT { get; set; } = LeaveRequestStatus.Pending;

    public virtual Employee Employee { get; set; } = null!;
}
