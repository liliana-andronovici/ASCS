using InfigoHR.Application.DTOs;
namespace InfigoHR.Application.Interfaces;
public interface IAuthService
{
    Task<IReadOnlyList<AuthDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthDto?> GetByIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<AuthDto> CreateAsync(CreateAuthDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int employeeId, UpdateAuthDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int employeeId, CancellationToken cancellationToken = default);
}
