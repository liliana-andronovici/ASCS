using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Services;

public class LocationRepository : ILocationRepository
{
    private readonly AppDbContext _context;

    public LocationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LocationEntity>> GetAllAsync()
        => await _context.Locations.ToListAsync();

    public async Task<LocationEntity?> GetByIdAsync(int id)
        => await _context.Locations.FindAsync(id);

    public async Task AddAsync(LocationEntity location)
        => await _context.Locations.AddAsync(location);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Remove(LocationEntity location)
        => _context.Locations.Remove(location);
}