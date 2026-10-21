using InfigoHR.Application.Models.LeaveRequests;

namespace InfigoHR.Application.Interfaces.Services;

public interface ILeaveRequestService
{
    Task<IReadOnlyList<LeaveRequestModel>> GetAllAsync();
    Task<LeaveRequestModel> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateLeaveRequestModel model);
    Task UpdateAsync(UpdateLeaveRequestModel model);
    Task DeleteAsync(int id);
}