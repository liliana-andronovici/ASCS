using InfigoHR.Application.DTOs;
using InfigoHR.Application.Interfaces;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Services;

public sealed class EmployeeService(AppDbContext dbContext) : IEmployeeService
{
    // pentru a obtine toti angajatii, trebuie sa trimitem un GET request catre endpoint json
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Employees.AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new EmployeeDto(
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.Title,
                x.StartedOnUtc,
                x.DepartmentId,
                x.LocationId,
                x.ManagerId,
                x.Status))
            .ToListAsync(cancellationToken);

    // pentru a obtine un angajat dupa id, trebuie sa trimitem un GET request catre endpoint json cu id-ul angajatului
    public async Task<EmployeeDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Employees.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new EmployeeDto(
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.Title,
                x.StartedOnUtc,
                x.DepartmentId,
                x.LocationId,
                x.ManagerId,
                x.Status))
            .SingleOrDefaultAsync(cancellationToken);

    // pentru a crea un nou angajat, trebuie sa trimitem un POST request catre endpoint json cu datele angajatului
    public async Task<EmployeeDto> CreateAsync(
        CreateEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Title = dto.Title,
            StartedOnUtc = dto.StartedOnUtc,
            DepartmentId = dto.DepartmentId,
            LocationId = dto.LocationId,
            ManagerId = dto.ManagerId,
            Status = dto.Status
        };
        dbContext.Employees.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    // pentru a actualiza un angajat, trebuie sa trimitem un PUT request catre endpoint json cu id-ul si datele noi
    public async Task<bool> UpdateAsync(
        int id,
        UpdateEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Employees.SingleOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);
        if (entity is null)
        {
            return false;
        }
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Email = dto.Email;
        entity.Title = dto.Title;
        entity.StartedOnUtc = dto.StartedOnUtc;
        entity.DepartmentId = dto.DepartmentId;
        entity.LocationId = dto.LocationId;
        entity.ManagerId = dto.ManagerId;
        entity.Status = dto.Status;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Employees.SingleOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);
        if (entity is null)
        {
            return false;
        }
        dbContext.Employees.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static EmployeeDto Map(Employee entity) =>
        new(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.Email,
            entity.Title,
            entity.StartedOnUtc,
            entity.DepartmentId,
            entity.LocationId,
            entity.ManagerId,
            entity.Status);
}
