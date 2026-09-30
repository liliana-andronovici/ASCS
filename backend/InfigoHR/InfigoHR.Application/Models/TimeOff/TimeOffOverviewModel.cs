namespace InfigoHR.Application.Models.TimeOff;

public class TimeOffOverviewModel
{
    public string PolicyName { get; set; }
    public DateTime ResetsOn { get; set; }
    public string ApproverName { get; set; }
    public List<TimeOffBalanceModel> Balances { get; set; }
}