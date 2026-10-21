using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface ILeaveRequestRepository : IGenericRepository<LeaveRequestEntity>
{
    Task<IReadOnlyList<LeaveRequestEntity>> GetByEmployeeIdAsync(int employeeId);
}