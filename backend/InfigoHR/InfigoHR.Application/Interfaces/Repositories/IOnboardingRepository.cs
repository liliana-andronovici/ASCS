using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IOnboardingRepository : IGenericRepository<OnboardingEntity>
{
    Task<OnboardingEntity?> GetByEmployeeIdAsync(int employeeId);
    Task<OnboardingTaskEntity?> GetTaskByIdAsync(int taskId);
    Task AddTaskAsync(OnboardingTaskEntity task);
    Task UpdateTaskAsync(OnboardingTaskEntity task);
}