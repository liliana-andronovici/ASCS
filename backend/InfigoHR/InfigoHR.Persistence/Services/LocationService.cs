using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Services;

public sealed class LocationService(AppDbContext dbContext) : ILocationService
{
    public async Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Locations.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new LocationDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);

    public async Task<LocationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Locations.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new LocationDto(x.Id, x.Name))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<LocationDto> CreateAsync(
        CreateLocationDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = new Location { Name = dto.Name };
        dbContext.Locations.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new LocationDto(entity.Id, entity.Name);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateLocationDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Locations.SingleOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);
        if (entity is null)
        {
            return false;
        }
        entity.Name = dto.Name;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Locations.SingleOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);
        if (entity is null)
        {
            return false;
        }
        dbContext.Locations.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
