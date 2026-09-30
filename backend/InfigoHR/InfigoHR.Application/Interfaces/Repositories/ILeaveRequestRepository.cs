using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface ILeaveRequestRepository : IGenericRepository<LeaveRequestEntity>
{
    Task<List<LeaveRequestEntity>> GetForEmployeeAsync(int employeeId);
    Task<List<LeaveRequestEntity>> GetOverlappingAsync(int excludeEmployeeId, int? departmentId, DateTime start, DateTime end);
}