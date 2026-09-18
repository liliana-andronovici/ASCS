namespace InfigoHR.Application.Common;

public interface IPagedResult<T>
{
    List<T> Items { get; set; }
    int TotalCount { get; set; }
    int Page { get; set; }
    int PageSize { get; set; }
}