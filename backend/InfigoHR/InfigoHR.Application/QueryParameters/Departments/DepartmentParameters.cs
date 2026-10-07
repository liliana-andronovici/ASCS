using InfigoHR.Application.Common;

namespace InfigoHR.Application.QueryParameters.Departments;

public class DepartmentParameters : Parameters
{
    public string? Search { get; set; }
    public DepartmentSortBy? SortBy { get; set; }
}