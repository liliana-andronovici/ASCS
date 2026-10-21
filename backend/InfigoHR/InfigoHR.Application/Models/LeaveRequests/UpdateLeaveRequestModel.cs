using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models.LeaveRequests;

public class UpdateLeaveRequestModel
{
    public int Id { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string Note { get; set; } = string.Empty;
    public LeaveType Type { get; set; }
}
