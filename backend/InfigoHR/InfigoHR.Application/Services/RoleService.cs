using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.Roles;

namespace InfigoHR.Application.Services;

public class RoleService(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    IValidator<CreateRoleModel> createValidator,
    IValidator<UpdateRoleModel> updateValidator,
    IValidator<SetRolePermissionsModel> setPermissionsValidator,
    IValidator<RolePermissionModel> rolePermissionValidator) : IRoleService
{
    public async Task<List<RoleModel>> GetAllAsync()
    {
        var roles = await roleRepository.GetAllWithPermissionsAsync();
        return roles.Select(RoleMapper.ToModel).ToList();
    }

    public async Task<int> CreateAsync(CreateRoleModel model)
    {
        await createValidator.ValidateAndThrowAsync(model);

        if (await roleRepository.GetByNameAsync(model.Name) != null)
        {
            throw new ConflictException($"Role '{model.Name}' already exists.");
        }

        var role = RoleMapper.ToEntity(model);
        await roleRepository.AddAsync(role);
        return role.Id;
    }

    public async Task UpdateAsync(UpdateRoleModel model)
    {
        await updateValidator.ValidateAndThrowAsync(model);

        var role = await roleRepository.GetByIdAsync(model.Id)
            ?? throw new NotFoundException($"Role with id {model.Id} was not found.");

        var existing = await roleRepository.GetByNameAsync(model.Name);
        if (existing != null && existing.Id != model.Id)
        {
            throw new ConflictException($"Role '{model.Name}' already exists.");
        }

        RoleMapper.ToEntity(model, role);
        await roleRepository.UpdateAsync(role);
    }

    public async Task DeleteAsync(int id)
    {
        var role = await roleRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Role with id {id} was not found.");

        await roleRepository.DeleteAsync(role);
    }

    public async Task SetPermissionsAsync(SetRolePermissionsModel model)
    {
        await setPermissionsValidator.ValidateAndThrowAsync(model);

        var role = await roleRepository.GetWithPermissionsAsync(model.RoleId)
            ?? throw new NotFoundException($"Role with id {model.RoleId} was not found.");

        var permissions = new List<Domain.Entities.PermissionEntity>();
        foreach (var name in model.Permissions.Distinct())
        {
            var permission = await permissionRepository.GetByNameAsync(name)
                ?? throw new NotFoundException($"Permission '{name}' was not found.");
            permissions.Add(permission);
        }

        role.Permissions.Clear();
        foreach (var permission in permissions)
        {
            role.Permissions.Add(permission);
        }

        await roleRepository.UpdateAsync(role);
    }

    public async Task AddPermissionAsync(RolePermissionModel model)
    {
        await rolePermissionValidator.ValidateAndThrowAsync(model);

        var role = await roleRepository.GetWithPermissionsAsync(model.RoleId)
            ?? throw new NotFoundException($"Role with id {model.RoleId} was not found.");

        var permission = await permissionRepository.GetByNameAsync(model.PermissionName)
            ?? throw new NotFoundException($"Permission '{model.PermissionName}' was not found.");

        if (role.Permissions.Any(p => p.Id == permission.Id))
        {
            throw new ConflictException(
                $"Role '{role.Name}' already has permission '{permission.Name}'.");
        }

        role.Permissions.Add(permission);
        await roleRepository.UpdateAsync(role);
    }

    public async Task RemovePermissionAsync(RolePermissionModel model)
    {
        await rolePermissionValidator.ValidateAndThrowAsync(model);

        var role = await roleRepository.GetWithPermissionsAsync(model.RoleId)
            ?? throw new NotFoundException($"Role with id {model.RoleId} was not found.");

        var permission = role.Permissions.FirstOrDefault(p => p.Name == model.PermissionName)
            ?? throw new NotFoundException(
                $"Role '{role.Name}' does not have permission '{model.PermissionName}'.");

        role.Permissions.Remove(permission);
        await roleRepository.UpdateAsync(role);
    }
}
