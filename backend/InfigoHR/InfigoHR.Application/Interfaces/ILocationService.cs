using InfigoHR.Application.DTOs;

namespace InfigoHR.Application.Interfaces;

public interface ILocationService
{
    Task<List<LocationModel>> GetAllAsync();
    Task<LocationModel> GetByIdAsync(int id);
    Task<LocationModel> CreateAsync(CreateLocationModel model);
    Task<LocationModel> UpdateAsync(int id, UpdateLocationModel model);
    Task DeleteAsync(int id);
}