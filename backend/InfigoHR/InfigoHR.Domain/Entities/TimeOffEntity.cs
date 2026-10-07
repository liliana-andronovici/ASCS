using InfigoHR.Domain.Common;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Domain.Entities;

public class TimeOffEntity : BaseEntity
{
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string Note { get; set; }
    public TimeOffType Type { get; set; }
    public TimeOffStatus Status { get; set; }

    public int EmployeeId { get; set; }
    public virtual EmployeeEntity? Employee { get; set; }
}
