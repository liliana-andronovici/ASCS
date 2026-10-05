using InfigoHR.Application.Models;

namespace InfigoHR.Application.Interfaces.Services;

public interface IOnboardingService
{
    Task<OnboardingChecklistModel> GetChecklistAsync(int employeeId);
    Task AddTaskAsync(int employeeId, AddOnboardingTaskModel model);
    Task ToggleTaskAsync(int employeeId, int taskId);
    Task<List<OnboardingSummaryModel>> GetOnboardingSummariesAsync();
}