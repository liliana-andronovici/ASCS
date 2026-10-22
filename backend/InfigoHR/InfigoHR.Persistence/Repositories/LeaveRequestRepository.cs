using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;
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

    public Task<int> CountApprovedOnAsync(DateOnly date)
    {
        var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = start.AddDays(1);
        return Entities.AsNoTracking()
            .Where(request => request.Status == LeaveRequestStatus.Approved &&
                              request.StartDateUtc < end && request.EndDateUtc >= start)
            .Select(request => request.EmployeeId)
            .Distinct()
            .CountAsync();
    }
}
