namespace InfigoHR.Application.Common;

public class PagedResult<T> : IPagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}