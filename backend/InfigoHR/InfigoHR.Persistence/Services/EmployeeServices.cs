using InfigoHR.Application.Models;
using InfigoHR.Application.Interfaces;
using InfigoHR.Application.Mappers;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Exceptions;

namespace InfigoHR.Persistence.Services;

public sealed class EmployeeService(IEmployeeRepository repository) : IEmployeeService
{
    public async Task<List<EmployeeModel>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return entities.Select(EmployeeMapper.ToDto).ToList();
    }

    public async Task<EmployeeModel> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee with id {id} was not found.");
        return EmployeeMapper.ToDto(entity);
    }

    public async Task<EmployeeModel> CreateAsync(CreateEmployeeModel dto)
    {
        var duplicates = await repository.GetAllAsync();
        if (duplicates.Any(x => x.Email == dto.Email))
            throw new ConflictException($"An employee with email '{dto.Email}' already exists.");

        if (dto.DepartmentId is not null && !await repository.DepartmentExistsAsync(dto.DepartmentId.Value))
            throw new ValidationException("The specified department does not exist.");

        if (dto.LocationId is not null && !await repository.LocationExistsAsync(dto.LocationId.Value))
            throw new ValidationException("The specified location does not exist.");

        if (dto.ManagerId is not null && await repository.GetByIdAsync(dto.ManagerId.Value) is null)
            throw new ValidationException("The specified manager does not exist.");

        var entity = new EmployeeEntity
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
        await repository.AddAsync(entity);
        return EmployeeMapper.ToDto(entity);
    }
    
    public async Task UpdateAsync(int id, UpdateEmployeeModel dto)
    {
        var entity = await repository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"Employee with id {id} was not found.");

        if (dto.DepartmentId is not null && !await repository.DepartmentExistsAsync(dto.DepartmentId.Value))
            throw new ValidationException("The specified department does not exist.");

        if (dto.LocationId is not null && !await repository.LocationExistsAsync(dto.LocationId.Value))
            throw new ValidationException("The specified location does not exist.");

        if (dto.ManagerId is not null && await repository.GetByIdAsync(dto.ManagerId.Value) is null)
            throw new ValidationException("The specified manager does not exist.");

        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Email = dto.Email;
        entity.Title = dto.Title;
        entity.StartedOnUtc = dto.StartedOnUtc;
        entity.DepartmentId = dto.DepartmentId;
        entity.LocationId = dto.LocationId;
        entity.ManagerId = dto.ManagerId;
        entity.Status = dto.Status;

        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee with id {id} was not found.");
        await repository.DeleteAsync(entity);
    }
}