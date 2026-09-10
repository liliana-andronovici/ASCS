using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Services;

public sealed class EmployeeService(AppDbContext dbContext) : IEmployeeService
{
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Employees.AsNoTracking()
            .OrderBy(e => e.EmployeeId)
            .Select(e => new EmployeeDto(e.EmployeeId, e.FirstName, e.LastName, e.Email, e.Title, e.Started, e.DepartmentId, e.LocationId, e.ManagerId, e.Status))
            .ToListAsync(cancellationToken);

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Employees.AsNoTracking()
            .Where(e => e.EmployeeId == id)
            .Select(e => new EmployeeDto(e.EmployeeId, e.FirstName, e.LastName, e.Email, e.Title, e.Started, e.DepartmentId, e.LocationId, e.ManagerId, e.Status))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Title = dto.Title,
            Started = dto.Started,
            DepartmentId = dto.DepartmentId,
            LocationId = dto.LocationId,
            ManagerId = dto.ManagerId,
            Status = dto.Status
        };
        dbContext.Employees.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new EmployeeDto(entity.EmployeeId, entity.FirstName, entity.LastName, entity.Email, entity.Title, entity.Started, entity.DepartmentId, entity.LocationId, entity.ManagerId, entity.Status);
    }

    public async Task<bool> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Employees.SingleOrDefaultAsync(e => e.EmployeeId == id, cancellationToken);
        if (entity is null) return false;
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Email = dto.Email;
        entity.Title = dto.Title;
        entity.Started = dto.Started;
        entity.DepartmentId = dto.DepartmentId;
        entity.LocationId = dto.LocationId;
        entity.ManagerId = dto.ManagerId;
        entity.Status = dto.Status;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Employees.SingleOrDefaultAsync(e => e.EmployeeId == id, cancellationToken);
        if (entity is null) return false;
        dbContext.Employees.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
