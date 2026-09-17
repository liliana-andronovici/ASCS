using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InfigoHR.Application.Common;

namespace InfigoHR.Persistence.Repositories;

public class LocationRepository : GenericRepository<LocationEntity>, ILocationRepository
{
    public LocationRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task UpdateAsync(LocationEntity entity)
    {
        entity.UpdatedOnUtc = DateTime.UtcNow;
        await base.UpdateAsync(entity);
    }

    public async Task<PagedResult<LocationEntity>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = Entities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(paginationParams.Search))
        {
            query = query.Where(l => EF.Functions.ILike(l.Name, $"%{paginationParams.Search}%"));
        }

        var totalCount = await query.CountAsync();

        query = paginationParams.SortBy switch
        {
            LocationSortBy.Name => paginationParams.SortDirection == SortDirection.Descending
                ? query.OrderByDescending(l => l.Name)
                : query.OrderBy(l => l.Name),
            _ => paginationParams.SortDirection == SortDirection.Descending
                ? query.OrderByDescending(l => l.Id)
                : query.OrderBy(l => l.Id)
        };

        var items = await query
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return new PagedResult<LocationEntity>
        {
            Items = items,
            TotalCount = totalCount,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };
    }
}