using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.Permissions;

namespace InfigoHR.Application.Services;

public class PermissionService(
    IPermissionRepository repository,
    IValidator<CreatePermissionModel> createValidator,
    IValidator<UpdatePermissionModel> updateValidator) : IPermissionService
{
    public async Task<List<PermissionModel>> GetAllAsync()
    {
        var permissions = await repository.GetAllAsync();
        return permissions.Select(PermissionMapper.ToModel).OrderBy(p => p.Name).ToList();
    }

    public async Task<int> GetIdByNameAsync(string name)
    {
        var permission = await repository.GetByNameAsync(name)
            ?? throw new NotFoundException($"Permission '{name}' was not found.");

        return permission.Id;
    }

    public async Task<int> CreateAsync(CreatePermissionModel model)
    {
        await createValidator.ValidateAndThrowAsync(model);

        if (await repository.GetByNameAsync(model.Name) != null)
        {
            throw new ConflictException($"Permission '{model.Name}' already exists.");
        }

        var permission = PermissionMapper.ToEntity(model);
        await repository.AddAsync(permission);
        return permission.Id;
    }

    public async Task UpdateAsync(UpdatePermissionModel model)
    {
        await updateValidator.ValidateAndThrowAsync(model);

        var permission = await repository.GetByIdAsync(model.Id)
            ?? throw new NotFoundException($"Permission with id {model.Id} was not found.");

        if (permission.Name == "ManageRoles")
            throw new ConflictException("ManageRoles cannot be renamed.");

        var existing = await repository.GetByNameAsync(model.Name);
        if (existing != null && existing.Id != model.Id)
        {
            throw new ConflictException($"Permission '{model.Name}' already exists.");
        }

        PermissionMapper.ToEntity(model, permission);
        await repository.UpdateAsync(permission);
    }

    public async Task DeleteAsync(int id)
    {
        var permission = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Permission with id {id} was not found.");

        if (permission.Name == "ManageRoles")
            throw new ConflictException("ManageRoles cannot be deleted.");

        await repository.DeleteAsync(permission);
    }
}
