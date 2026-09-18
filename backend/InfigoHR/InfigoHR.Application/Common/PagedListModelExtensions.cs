using InfigoHR.Domain.Common;

namespace InfigoHR.Application.Common;

public static class PagedListModelExtensions
{
    public static PagedListModel<TModel> ToPagedModel<TEntity, TModel>(
        this PagedList<TEntity> pagedList,
        Func<TEntity, TModel> mapper)
    {
        return new PagedListModel<TModel>
        {
            Items = pagedList.Items.Select(mapper).ToList(),
            TotalCount = pagedList.TotalCount,
            Page = pagedList.Page,
            PageSize = pagedList.PageSize
        };
    }
}