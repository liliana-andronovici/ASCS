namespace InfigoHR.Application.Common;

public class PaginationParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? SortBy { get; set; } // todo use enums
    public bool SortDescending { get; set; } = false; // todo use enums
}