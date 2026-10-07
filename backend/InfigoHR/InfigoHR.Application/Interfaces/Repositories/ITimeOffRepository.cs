using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface ITimeOffRepository : IGenericRepository<TimeOffEntity>
{
    Task<IReadOnlyList<TimeOffEntity>> GetForEmployeeAsync(int employeeId);
    Task<TimeOffEntity?> GetForEmployeeByIdAsync(int id, int employeeId);
}
