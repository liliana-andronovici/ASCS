using InfigoHR.Application.Models.TimeOff;

namespace InfigoHR.Application.Interfaces.Services;

public interface ITimeOffService
{
    Task<TimeOffOverviewModel> GetOverviewAsync(int employeeId);
    Task<List<TimeOffRequestModel>> GetMyRequestsAsync(int employeeId);
    Task<TimeOffPreviewModel> PreviewAsync(int employeeId, TimeOffPreviewInputModel model);
    Task<int> CreateAsync(int employeeId, CreateTimeOffRequestModel model);
    Task WithdrawAsync(int employeeId, int id);
}