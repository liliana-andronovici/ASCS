using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Domain.Common;

public static class PagedListExtensions
{
    public static async Task<PagedList<T>> ToPagedAsync<T>(this IQueryable<T> queryable, int page, int pageSize)
    {
        var totalCount = await queryable.CountAsync();

        var items = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}