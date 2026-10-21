using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class LeaveRequestRepository : GenericRepository<LeaveRequestEntity>, ILeaveRequestRepository
{
    public LeaveRequestRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<LeaveRequestEntity>> GetByEmployeeIdAsync(int employeeId) =>
        await Entities.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderBy(x => x.StartDateUtc)
            .ThenBy(x => x.Id)
            .ToListAsync();
}
