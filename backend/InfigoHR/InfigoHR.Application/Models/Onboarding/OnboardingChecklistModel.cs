namespace InfigoHR.Application.Models;

public class OnboardingChecklistModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartsOn { get; set; }
    public string ManagerName { get; set; } = string.Empty;
    public string BuddyName { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
    public List<OnboardingSectionModel> Sections { get; set; } = new();
}