using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models.TimeOff;

public class TimeOffPreviewInputModel
{
    public LeaveType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class TimeOffPreviewModel
{
    public int WorkingDays { get; set; }
    public int? RemainingAfter { get; set; }
    public List<string> OverlappingPeople { get; set; }
}