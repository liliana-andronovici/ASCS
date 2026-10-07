using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class TimeOffRepository : GenericRepository<TimeOffEntity>, ITimeOffRepository
{
    public TimeOffRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<TimeOffEntity>> GetForEmployeeAsync(int employeeId) =>
        await Entities.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderBy(x => x.StartDateUtc)
            .ThenBy(x => x.Id)
            .ToListAsync();

    public Task<TimeOffEntity?> GetForEmployeeByIdAsync(int id, int employeeId) =>
        Entities.FirstOrDefaultAsync(x => x.Id == id && x.EmployeeId == employeeId);
}
