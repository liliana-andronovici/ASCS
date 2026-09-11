using InfigoHR.Application.DTOs;

namespace InfigoHR.Application.Interfaces;

public interface IAuthService
{
    Task<IReadOnlyList<AuthDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AuthDto> CreateAsync(CreateAuthDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateAuthDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
