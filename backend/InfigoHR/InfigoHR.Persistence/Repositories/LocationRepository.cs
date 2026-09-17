using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InfigoHR.Application.Common;

namespace InfigoHR.Persistence.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly AppDbContext _context;
    private DbSet<LocationEntity> Locations => _context.Locations;

    public LocationRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<LocationEntity>> GetAllAsync()
        => await Locations.ToListAsync();

    public async Task<LocationEntity?> GetByIdAsync(int id)
        => await Locations.FindAsync(id);

    public async Task AddAsync(LocationEntity location)
    {
        await Locations.AddAsync(location);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LocationEntity entity)
    {
        entity.UpdatedOnUtc = DateTime.UtcNow;
        Locations.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(LocationEntity location)
    {
        Locations.Remove(location);
        await _context.SaveChangesAsync();
    }

    public async Task<PagedResult<LocationEntity>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = Locations.AsQueryable();

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