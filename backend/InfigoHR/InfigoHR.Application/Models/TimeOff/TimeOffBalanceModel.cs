using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models.TimeOff;

public class TimeOffHeadlineModel
{
    public int Value { get; set; }
    public string Label { get; set; }
}

public class TimeOffBalanceModel
{
    public LeaveType Type { get; set; }
    public string Label { get; set; }
    public TimeOffHeadlineModel Headline { get; set; }
    public int Used { get; set; }
    public int Pending { get; set; }
    public int? Total { get; set; }
    public string Caption { get; set; }
}