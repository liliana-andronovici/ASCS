using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models;

public  class CreateEmployeeModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }
    public DateTime StartedOnUtc { get; set; }
    public int? DepartmentId { get; set; }
    public int? LocationId { get; set; }
    public int? ManagerId { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Inactive;
}