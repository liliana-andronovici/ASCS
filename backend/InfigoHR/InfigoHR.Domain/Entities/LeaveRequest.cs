using InfigoHR.Domain.Common;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Domain.Entities;

public class LeaveRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public required string Note { get; set; }
    public LeaveType Type { get; set; }
    public LeaveRequestStatus StatusT { get; set; } = LeaveRequestStatus.Pending;
    public virtual Employee Employee { get; set; } = null!;
}
