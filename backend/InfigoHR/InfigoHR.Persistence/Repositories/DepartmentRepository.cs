using InfigoHR.Application.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

// changed: repository pattern added
public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _dbContext;

    public DepartmentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<DepartmentEntity>> GetAllAsync()
    {
        return await _dbContext.Departments
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<DepartmentEntity?> GetByIdAsync(int id, bool asNoTracking = false)
    {
        var query = _dbContext.Departments.AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(DepartmentEntity department)
    {
        await _dbContext.Departments.AddAsync(department);
    }

    public void Remove(DepartmentEntity department)
    {
        _dbContext.Departments.Remove(department);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
