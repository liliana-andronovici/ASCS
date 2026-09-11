using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.DTOs;

public sealed record EmployeeDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Title,
    DateTime StartedOnUtc,
    int? DepartmentId,
    int LocationId,
    int? ManagerId,
    EmployeeStatus Status);

public sealed record CreateEmployeeDto(
    string FirstName,
    string LastName,
    string Email,
    string Title,
    DateTime StartedOnUtc,
    int? DepartmentId,
    int LocationId,
    int? ManagerId,
    EmployeeStatus Status = EmployeeStatus.Inactive);

public sealed record UpdateEmployeeDto(
    string FirstName,
    string LastName,
    string Email,
    string Title,
    DateTime StartedOnUtc,
    int? DepartmentId,
    int LocationId,
    int? ManagerId,
    EmployeeStatus Status);
