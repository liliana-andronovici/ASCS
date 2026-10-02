namespace InfigoHR.Application.Models;

public class AddOnboardingTaskModel
{
    public string SectionKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? DueDate { get; set; }
}