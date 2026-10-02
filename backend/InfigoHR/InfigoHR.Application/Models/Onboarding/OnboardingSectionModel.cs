namespace InfigoHR.Application.Models;

public class OnboardingSectionModel
{
    public string Key { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<OnboardingTaskModel> Tasks { get; set; } = new();
}