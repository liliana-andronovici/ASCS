namespace InfigoHR.Application.Common;

public abstract class QueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
}