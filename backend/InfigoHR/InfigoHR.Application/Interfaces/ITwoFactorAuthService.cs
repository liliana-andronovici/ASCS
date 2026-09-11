using InfigoHR.Application.DTOs;

namespace InfigoHR.Application.Interfaces;

public interface ITwoFactorAuthService
{
    Task<IReadOnlyList<TwoFactorAuthDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TwoFactorAuthDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TwoFactorAuthDto> CreateAsync(CreateTwoFactorAuthDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateTwoFactorAuthDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
