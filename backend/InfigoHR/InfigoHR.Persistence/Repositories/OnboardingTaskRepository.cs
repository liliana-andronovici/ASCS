using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class OnboardingTaskRepository(AppDbContext dbContext)
    : GenericRepository<OnboardingTaskEntity>(dbContext), IOnboardingTaskRepository
{
    public Task<OnboardingTaskEntity?> GetByIdWithOnboardingAsync(int taskId) =>
        Entities
            .Include(task => task.Onboarding)
            .FirstOrDefaultAsync(task => task.Id == taskId);

    public Task<List<OnboardingTaskEntity>> GetUpcomingForEmployeeAsync(int employeeId, DateTime fromUtc, DateTime toUtc) =>
        Entities
            .AsNoTracking()
            .Where(task => task.Onboarding!.EmployeeId == employeeId && task.CompletedOnUtc == null &&
                           task.DueDateUtc >= fromUtc && task.DueDateUtc < toUtc)
            .OrderBy(task => task.DueDateUtc)
            .ToListAsync();
}
