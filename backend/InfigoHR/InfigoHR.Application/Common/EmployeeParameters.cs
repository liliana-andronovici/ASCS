using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Common;

public sealed class EmployeeParameters : QueryParameters
{
    public string? Search { get; set; }
    public int? DepartmentId { get; set; }
    public int? LocationId { get; set; }
    public EmployeeStatus? Status { get; set; }
    public EmployeeSortField SortBy { get; set; } = EmployeeSortField.Id;
}