using InfigoHR.Application.Models.LeaveRequests;
using InfigoHR.Application.Models.TimeOff;

namespace InfigoHR.Application.Interfaces.Services;

public interface ILeaveRequestService
{
    Task<int> GetOutTodayCountAsync(DateOnly date);
    Task<TimeOffOverviewModel> GetOverviewAsync();
    Task<IReadOnlyList<TimeOffRequestSummaryModel>> GetRequestSummariesAsync();
    Task<TimeOffPreviewModel> PreviewAsync(TimeOffPreviewInputModel model);
    Task<int> CreateFromPageAsync(TimeOffRequestInputModel model);
    Task WithdrawAsync(int id);
    Task<IReadOnlyList<LeaveRequestModel>> GetAllAsync();
    Task<LeaveRequestModel> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateLeaveRequestModel model);
    Task UpdateAsync(UpdateLeaveRequestModel model);
    Task DeleteAsync(int id);
}
