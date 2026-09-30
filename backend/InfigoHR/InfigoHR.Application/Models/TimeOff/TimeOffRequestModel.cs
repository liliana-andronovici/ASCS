using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models.TimeOff;

public class TimeOffRequestModel
{
    public int Id { get; set; }
    public LeaveType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Days { get; set; }
    public string ApproverName { get; set; }
    public LeaveRequestStatus Status { get; set; }
    public bool CanWithdraw { get; set; }
}