using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IOnboardingRepository : IGenericRepository<OnboardingEntity>
{
    Task<OnboardingEntity?> GetByEmployeeIdAsync(int employeeId);
    Task<OnboardingEntity?> GetByEmployeeIdWithDetailsAsync(int employeeId);
    Task<List<OnboardingEntity>> GetAllWithDetailsAsync();
}
