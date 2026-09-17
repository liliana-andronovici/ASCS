using InfigoHR.Application.Common;
using InfigoHR.Application.Models.Locations;
namespace InfigoHR.Application.Interfaces.Services;
public interface ILocationService
{
    Task<List<LocationModel>> GetAllAsync();
    Task<PagedResult<LocationModel>> GetPagedAsync(PaginationParams paginationParams);
    Task<LocationModel> GetByIdAsync(int id);
    Task CreateAsync(CreateLocationModel model);
    Task UpdateAsync(int id, UpdateLocationModel model);
    Task DeleteAsync(int id);
}