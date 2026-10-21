using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models.LeaveRequests;

public class LeaveRequestModel
{
    public int Id { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string Note { get; set; } = string.Empty;
    public LeaveType Type { get; set; }
    public LeaveRequestStatus Status { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
}
