using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class TimeOffRepository : GenericRepository<LeaveRequestEntity>, ITimeOffRepository
{
    public TimeOffRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<LeaveRequestEntity>> GetByEmployeeAsync(int employeeId) =>
        await Entities.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderBy(x => x.StartDateUtc)
            .ThenBy(x => x.Id)
            .ToListAsync();
}
