using InfigoHR.Application.DTOs;

namespace InfigoHR.Application.Interfaces;

public interface ITrustedDeviceService
{
    Task<IReadOnlyList<TrustedDeviceDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TrustedDeviceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TrustedDeviceDto> CreateAsync(CreateTrustedDeviceDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateTrustedDeviceDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
