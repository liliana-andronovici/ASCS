using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class LeaveRequestRepository(AppDbContext dbContext)
    : GenericRepository<LeaveRequestEntity>(dbContext), ILeaveRequestRepository
{
    public async Task<List<LeaveRequestEntity>> GetForEmployeeAsync(int employeeId) =>
        await Entities.Where(x => x.EmployeeId == employeeId).ToListAsync();

    public async Task<List<LeaveRequestEntity>> GetOverlappingAsync(
        int excludeEmployeeId, int? departmentId, DateTime start, DateTime end)
    {
        if (departmentId is null)
        {
            return [];
        }

        return await Entities
            .Include(x => x.Employee)
            .Where(x => x.EmployeeId != excludeEmployeeId
                        && x.Employee!.DepartmentId == departmentId
                        && (x.Status == LeaveRequestStatus.Approved || x.Status == LeaveRequestStatus.Pending)
                        && x.StartDateUtc <= end && x.EndDateUtc >= start)
            .ToListAsync();
    }
}