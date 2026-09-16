using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Repositories;

// changed: repository abstraction added
public interface IDepartmentRepository
{
    Task<IEnumerable<DepartmentEntity>> GetAllAsync();
    Task<DepartmentEntity?> GetByIdAsync(int id, bool asNoTracking = false);
    Task AddAsync(DepartmentEntity department);
    void Remove(DepartmentEntity department);
    Task SaveChangesAsync();
}
