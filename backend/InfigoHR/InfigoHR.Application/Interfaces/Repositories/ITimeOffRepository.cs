using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface ITimeOffRepository : IGenericRepository<LeaveRequestEntity>
{
    Task<IReadOnlyList<LeaveRequestEntity>> GetByEmployeeAsync(int employeeId);
}