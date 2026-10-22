using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IOnboardingTaskRepository : IGenericRepository<OnboardingTaskEntity>
{
    Task<OnboardingTaskEntity?> GetByIdWithOnboardingAsync(int taskId);
    Task<List<OnboardingTaskEntity>> GetUpcomingForEmployeeAsync(int employeeId, DateTime fromUtc, DateTime toUtc);
}
