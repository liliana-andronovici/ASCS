using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Persistence.Repositories;

public class DepartmentRepository : GenericRepository<DepartmentEntity>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
