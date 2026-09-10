using InfigoHR.Application.DTOs;
namespace InfigoHR.Application.Interfaces;
public interface ILocationService
{
    Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LocationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LocationDto> CreateAsync(CreateLocationDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateLocationDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
