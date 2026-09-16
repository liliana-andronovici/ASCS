using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public sealed class EmployeeRepository(AppDbContext dbContext) : IEmployeeRepository
{
    public async Task<List<EmployeeEntity>> GetAllAsync() =>
        await dbContext.Employees.AsNoTracking().ToListAsync();

    public async Task<EmployeeEntity?> GetByIdAsync(int id) =>
        await dbContext.Employees.FindAsync(id);

    public async Task AddAsync(EmployeeEntity entity)
    {
        dbContext.Employees.Add(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(EmployeeEntity entity)
    {
        dbContext.Employees.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
    public async Task<bool> DepartmentExistsAsync(int id) =>
        await dbContext.Departments.AnyAsync(x => x.Id == id);

    public async Task<bool> LocationExistsAsync(int id) =>
        await dbContext.Locations.AnyAsync(x => x.Id == id);

    public async Task SaveChangesAsync() => await dbContext.SaveChangesAsync();
}