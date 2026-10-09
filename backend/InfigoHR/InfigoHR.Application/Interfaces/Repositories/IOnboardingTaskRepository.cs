using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IOnboardingTaskRepository : IGenericRepository<OnboardingTaskEntity>
{
    Task<OnboardingTaskEntity?> GetByIdWithOnboardingAsync(int taskId);
}