using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class EmployeeRepository
    : GenericRepository<EmployeeEntity>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public Task<EmployeeEntity?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return Entities
            .AsNoTracking()
            .Include(employee => employee.Auth)
            .Include(employee => employee.Roles)
            .ThenInclude(role => role.Permissions)
            .SingleOrDefaultAsync(
                employee => employee.Email == email,
                cancellationToken);
    }

    public Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return Entities.AnyAsync(
            employee => employee.Email == email,
            cancellationToken);
    }
}