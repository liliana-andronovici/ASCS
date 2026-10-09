using InfigoHR.Domain.Common;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Domain.Entities;

public class LeaveRequestEntity : BaseEntity
{
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string Note { get; set; }
    public LeaveType Type { get; set; }
    public LeaveRequestStatus Status { get; set; }
    public int EmployeeId { get; set; }
    public virtual EmployeeEntity? Employee { get; set; }
}
