using InfigoHR.Domain.Enums;

namespace InfigoHR.Application.Models;

public sealed class EmployeeParameters
{
    public string? Search { get; set; }
    public int? DepartmentId { get; set; }
    public int? LocationId { get; set; }
    public EmployeeStatus? Status { get; set; }
    public EmployeeSortField SortBy { get; set; } = EmployeeSortField.Id;
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}