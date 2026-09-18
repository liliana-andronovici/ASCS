using FluentValidation;
using InfigoHR.Application.Common;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models;
using InfigoHR.Domain.Exceptions;
using ValidationException = InfigoHR.Domain.Exceptions.ValidationException;

namespace InfigoHR.Application.Services;

public sealed class EmployeeService(
    IEmployeeRepository repository,
    IValidator<CreateEmployeeModel> createValidator,
    IValidator<UpdateEmployeeModel> updateValidator) : IEmployeeService
{
    public async Task<PagedResult<EmployeeModel>> GetAllAsync(EmployeeParameters query)
    {
        var (entities, totalCount) = await repository.SearchAsync(query);
        var items = entities.Select(EmployeeMapper.ToModel).ToList();
        return new PagedResult<EmployeeModel>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };    }

    public async Task<EmployeeModel> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee with id {id} was not found.");
        return EmployeeMapper.ToModel(entity);
    }

    public async Task CreateAsync(CreateEmployeeModel model)
    {
        await createValidator.ValidateAndThrowAsync(model);

        if (await repository.EmailExistsAsync(model.Email))
            throw new ConflictException($"An employee with email '{model.Email}' already exists.");

        if (model.DepartmentId is not null && !await repository.DepartmentExistsAsync(model.DepartmentId.Value))
            throw new ValidationException("The specified department does not exist.");

        if (model.LocationId is not null && !await repository.LocationExistsAsync(model.LocationId.Value))
            throw new ValidationException("The specified location does not exist.");

        if (model.ManagerId is not null && await repository.GetByIdAsync(model.ManagerId.Value) is null)
            throw new ValidationException("The specified manager does not exist.");

        var entity = EmployeeMapper.ToEntity(model);
        await repository.AddAsync(entity);
    }

    public async Task UpdateAsync(UpdateEmployeeModel model)
    {
        await updateValidator.ValidateAndThrowAsync(model);

        var entity = await repository.GetByIdAsync(model.Id)
            ?? throw new NotFoundException($"Employee with id {model.Id} was not found.");

        if (model.DepartmentId is not null && !await repository.DepartmentExistsAsync(model.DepartmentId.Value))
            throw new ValidationException("The specified department does not exist.");

        if (model.LocationId is not null && !await repository.LocationExistsAsync(model.LocationId.Value))
            throw new ValidationException("The specified location does not exist.");

        if (model.ManagerId is not null && await repository.GetByIdAsync(model.ManagerId.Value) is null)
            throw new ValidationException("The specified manager does not exist.");

        EmployeeMapper.ToEntity(model, entity);
        await repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Employee with id {id} was not found.");
        await repository.DeleteAsync(entity);
    }
}