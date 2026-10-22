namespace InfigoHR.Application.Models.Employees;

public class EmployeeCsvModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartedOnUtc { get; set; }
    public string? Department { get; set; }
    public string? Location { get; set; }
}