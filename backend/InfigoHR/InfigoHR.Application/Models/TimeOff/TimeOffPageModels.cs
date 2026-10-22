using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models.TimeOff;

public sealed class TimeOffOverviewModel
{
    public string ApproverName { get; init; } = string.Empty;
    public List<TimeOffBalanceModel> Balances { get; init; } = [];
}

public sealed class TimeOffBalanceModel
{
    public LeaveType Type { get; init; }
    public string Label { get; init; } = string.Empty;
    public TimeOffHeadlineModel Headline { get; init; } = new();
    public int Used { get; init; }
    public int Pending { get; init; }
    public int? Total { get; init; }
    public string Caption { get; init; } = string.Empty;
}

public sealed class TimeOffHeadlineModel
{
    public int Value { get; init; }
    public string Label { get; init; } = string.Empty;
}

public sealed class TimeOffRequestSummaryModel
{
    public int Id { get; init; }
    public LeaveType Type { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public int Days { get; init; }
    public string ApproverName { get; init; } = string.Empty;
    public LeaveRequestStatus Status { get; init; }
    public bool CanWithdraw { get; init; }
}

public class TimeOffPreviewInputModel
{
    public LeaveType Type { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
}

public sealed class TimeOffRequestInputModel : TimeOffPreviewInputModel
{
    public string Note { get; init; } = string.Empty;
}

public sealed class TimeOffPreviewModel
{
    public int WorkingDays { get; init; }
    public int? RemainingAfter { get; init; }
}

public sealed class WithdrawTimeOffModel
{
    public int Id { get; init; }
}
