using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class OnboardingRepository(AppDbContext dbContext)
    : GenericRepository<OnboardingEntity>(dbContext), IOnboardingRepository
{
    public async Task<OnboardingEntity?> GetByEmployeeIdAsync(int employeeId) =>
        await Entities.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);

    public async Task<OnboardingEntity?> GetByEmployeeIdWithDetailsAsync(int employeeId) =>
        await Entities
            .Include(x => x.Employee!).ThenInclude(e => e!.Department)
            .Include(x => x.Employee!).ThenInclude(e => e!.Location)
            .Include(x => x.Employee!).ThenInclude(e => e!.Manager)
            .Include(x => x.Buddy)
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
    
    public async Task<List<OnboardingEntity>> GetAllWithDetailsAsync() =>
        await Entities.Include(x => x.Tasks).ToListAsync();
}
