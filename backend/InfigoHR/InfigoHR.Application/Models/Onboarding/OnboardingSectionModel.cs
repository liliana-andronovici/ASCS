namespace InfigoHR.Application.Models;

public class OnboardingSectionModel
{
    public OnboardingSectionKey Key { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<OnboardingTaskModel> Tasks { get; set; } = new();
}
