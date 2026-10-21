namespace InfigoHR.Application.Models;

public class OnboardingSummaryModel
{
    public int EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime StartsOn { get; set; }
    public int CompletedTaskCount { get; set; }
    public int TotalTaskCount { get; set; }
    public bool IsComplete { get; set; }
}