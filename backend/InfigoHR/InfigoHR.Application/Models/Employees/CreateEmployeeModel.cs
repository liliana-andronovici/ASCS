using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models;

public sealed record CreateEmployeeModel(
    string FirstName,
    string LastName,
    string Email,
    string Title,
    DateTime StartedOnUtc,
    int? DepartmentId,
    int? LocationId,
    int? ManagerId,
    EmployeeStatus Status = EmployeeStatus.Inactive);