using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IDepartmentRepository : IGenericRepository<DepartmentEntity>
{
    Task<bool> ExistsAsync(int id);
}