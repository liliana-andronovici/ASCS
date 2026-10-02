using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class OnboardingRepository(AppDbContext dbContext)
    : GenericRepository<OnboardingEntity>(dbContext), IOnboardingRepository
{
    public async Task<OnboardingEntity?> GetByEmployeeIdAsync(int employeeId) =>
        await Entities
            .Include(x => x.Employee!).ThenInclude(e => e!.Department)
            .Include(x => x.Employee!).ThenInclude(e => e!.Location)
            .Include(x => x.Employee!).ThenInclude(e => e!.Manager)
            .Include(x => x.Buddy)
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);

    public async Task<OnboardingTaskEntity?> GetTaskByIdAsync(int taskId) =>
        await Context.Set<OnboardingTaskEntity>()
            .Include(x => x.Onboarding)
            .FirstOrDefaultAsync(x => x.Id == taskId);

    public async Task AddTaskAsync(OnboardingTaskEntity task)
    {
        await Context.Set<OnboardingTaskEntity>().AddAsync(task);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateTaskAsync(OnboardingTaskEntity task)
    {
        task.UpdatedOnUtc = DateTime.UtcNow;
        Context.Set<OnboardingTaskEntity>().Update(task);
        await Context.SaveChangesAsync();
    }
}