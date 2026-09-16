using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces;

public interface ILocationRepository
{
    Task<List<LocationEntity>> GetAllAsync();
    Task<LocationEntity?> GetByIdAsync(int id);
    Task AddAsync(LocationEntity location);
    Task SaveChangesAsync();
    void Remove(LocationEntity location);
}