using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models.TimeOff;

public class TimeOffModel
{
    public int Id { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string Note { get; set; } = string.Empty;
    public TimeOffType Type { get; set; }
    public TimeOffStatus Status { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
}
