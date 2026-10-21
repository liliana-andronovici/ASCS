namespace InfigoHR.Application.Models;

public sealed class UpcomingOnboardingTaskModel
{
    public int Id { get; init; }
    public int EmployeeId { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateOnly DueDate { get; init; }
}
