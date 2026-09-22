using InfigoHR.Application.Common;
using InfigoHR.Application.Models;
using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.QueryParameters.Employees;

public class EmployeeParameters : Parameters
{
    public string? Search { get; set; }
    public int? DepartmentId { get; set; }
    public int? LocationId { get; set; }
    public EmployeeStatus? Status { get; set; }
    public EmployeeSortField SortBy { get; set; } = EmployeeSortField.Id;
}