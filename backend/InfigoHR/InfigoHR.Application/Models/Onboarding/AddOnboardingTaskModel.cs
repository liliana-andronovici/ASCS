namespace InfigoHR.Application.Models;

public class AddOnboardingTaskModel
{
    public OnboardingSectionKey? SectionKey { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string? DueDate { get; set; }
}
