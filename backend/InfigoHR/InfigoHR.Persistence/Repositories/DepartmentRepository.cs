using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class DepartmentRepository : GenericRepository<DepartmentEntity>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> ExistsAsync(int id) =>
        await Entities.AnyAsync(x => x.Id == id);
}