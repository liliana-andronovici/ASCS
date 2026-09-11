using InfigoHR.Application.DTOs;
namespace InfigoHR.Application.Interfaces;
public interface ILeaveRequestService
{
    Task<IReadOnlyList<LeaveRequestDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LeaveRequestDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateLeaveRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
