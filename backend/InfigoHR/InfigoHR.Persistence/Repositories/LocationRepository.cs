using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InfigoHR.Application.Common;
using InfigoHR.Domain.Common;
using InfigoHR.Persistence.Extensions;
using InfigoHR.Application.QueryParameters.Locations;

namespace InfigoHR.Persistence.Repositories;

public class LocationRepository : GenericRepository<LocationEntity>, ILocationRepository
{
    public LocationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedList<LocationEntity>> SearchAsync(LocationParameters paginationParams)
    {
        var query = Entities.AsQueryable();

        if (!string.IsNullOrWhiteSpace(paginationParams.Search))
        {
            query = query.Where(l => EF.Functions.ILike(l.Name, $"%{paginationParams.Search}%"));
        }

        query = ApplySorting(query, paginationParams);

        return await query.ToPagedListAsync(paginationParams.Page, paginationParams.PageSize);
    }

    private IQueryable<LocationEntity> ApplySorting(IQueryable<LocationEntity> query, LocationParameters paginationParams)
    {
        return paginationParams.SortBy switch
        {
            LocationSortBy.Name => paginationParams.SortDirection == SortDirection.Descending
                ? query.OrderByDescending(l => l.Name)
                : query.OrderBy(l => l.Name),
            _ => paginationParams.SortDirection == SortDirection.Descending
                ? query.OrderByDescending(l => l.Id)
                : query.OrderBy(l => l.Id)
        };
    }
}