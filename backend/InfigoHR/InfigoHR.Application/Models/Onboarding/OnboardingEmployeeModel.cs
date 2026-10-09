namespace InfigoHR.Application.Models;

public class OnboardingEmployeeModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime StartedOnUtc { get; set; }
}