using InfigoHR.Domain.Common;

namespace InfigoHR.Application.Common;

public static class PagedResultExtensions
{
    public static PagedResult<TModel> ToPagedResult<TEntity, TModel>(
        this PagedList<TEntity> pagedList,
        Func<TEntity, TModel> map) =>
        new()
        {
            Items = pagedList.Items.Select(map).ToList(),
            TotalCount = pagedList.TotalCount,
            Page = pagedList.Page,
            PageSize = pagedList.PageSize
        };
}