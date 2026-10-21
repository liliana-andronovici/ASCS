namespace InfigoHR.Application.Models;

public class OnboardingTaskModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedOn { get; set; }
}