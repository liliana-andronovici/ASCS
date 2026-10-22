using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models.Employees;

public class EmployeeModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartedOnUtc { get; set; }
    public int? DepartmentId { get; set; }
    public int? LocationId { get; set; }
    public int? ManagerId { get; set; }
    public EmployeeStatus Status { get; set; }
    public string? DepartmentName { get; set; }
    public string? LocationName { get; set; }
    public EmployeeMainDataModel? Manager { get; set; }
}