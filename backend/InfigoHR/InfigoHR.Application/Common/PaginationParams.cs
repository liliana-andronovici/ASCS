namespace InfigoHR.Application.Common;

public class PaginationParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public LocationSortBy? SortBy { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
}