using InfigoHR.Application.DTOs;
namespace InfigoHR.Application.Interfaces;
public interface ITrustedDeviceService
{
    Task<IReadOnlyList<TrustedDeviceDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TrustedDeviceDto?> GetByIdAsync(string deviceToken, CancellationToken cancellationToken = default);
    Task<TrustedDeviceDto> CreateAsync(CreateTrustedDeviceDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string deviceToken, UpdateTrustedDeviceDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string deviceToken, CancellationToken cancellationToken = default);
}
