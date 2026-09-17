using InfigoHR.Application.Common;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface ILocationRepository
{
    Task<List<LocationEntity>> GetAllAsync();
    Task<PagedResult<LocationEntity>> GetPagedAsync(PaginationParams paginationParams);
    Task<LocationEntity?> GetByIdAsync(int id);
    Task AddAsync(LocationEntity location);
    Task UpdateAsync(LocationEntity location);
    Task DeleteAsync(LocationEntity location);
}