using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models.TimeOff;

public class CreateTimeOffRequestModel
{
    public LeaveType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Note { get; set; }
}