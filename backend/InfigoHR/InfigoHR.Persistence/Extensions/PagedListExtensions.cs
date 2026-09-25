using InfigoHR.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Extensions;

public static class PagedListExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> queryable, int pageIndex, int pageSize)
    {
        var totalCount = await queryable.CountAsync();

        var items = await queryable
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = pageIndex,
            PageSize = pageSize
        };
    }
}