using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace InfigoHR.Persistence.Repositories;

public sealed class AuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<EmployeeEntity?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _context.Employees
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
        return _context.Employees.AnyAsync(
            employee => employee.Email == email,
            cancellationToken);
    }

    public async Task<bool> TryAddAsync(
        EmployeeEntity employee,
        CancellationToken cancellationToken = default)
    {
        var defaultRole = await _context.Set<RoleEntity>()
            .Include(role => role.Permissions)
            .SingleAsync(
                role => role.Id == RoleEntity.EmployeeRoleId,
                cancellationToken);

        employee.Roles.Add(defaultRole);
        _context.Employees.Add(employee);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException postgres &&
            postgres.SqlState == PostgresErrorCodes.UniqueViolation &&
            postgres.ConstraintName == "Employees_Email_key")
        {
            return false;
        }
    }
}