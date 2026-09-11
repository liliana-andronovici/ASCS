using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Services;

public sealed class DepartmentService(AppDbContext dbContext) : IDepartmentService
{
    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Departments.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new DepartmentDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);

    public async Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await dbContext.Departments.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new DepartmentDto(x.Id, x.Name))
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<DepartmentDto> CreateAsync(
        CreateDepartmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = new Department { Name = dto.Name };
        dbContext.Departments.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DepartmentDto(entity.Id, entity.Name);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateDepartmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Departments.SingleOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);
        if (entity is null)
        {
            return false;
        }
        entity.Name = dto.Name;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Departments.SingleOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);
        if (entity is null)
        {
            return false;
        }
        dbContext.Departments.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
