using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models.TimeOff;

public class CreateTimeOffModel
{
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string Note { get; set; } = string.Empty;
    public TimeOffType Type { get; set; }
}
