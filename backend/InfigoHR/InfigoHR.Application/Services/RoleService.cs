using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.Roles;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Services;

public class RoleService(
    IRoleRepository roleRepository,
    IEmployeeRepository employeeRepository,
    IPermissionRepository permissionRepository,
    IValidator<CreateRoleModel> createValidator,
    IValidator<UpdateRoleModel> updateValidator,
    IValidator<SetRolePermissionsModel> setPermissionsValidator,
    IValidator<RolePermissionModel> rolePermissionValidator) : IRoleService
{
    public async Task<List<RoleModel>> GetAllAsync()
    {
        var roles = await roleRepository.GetAllWithPermissionsAsync();
        var employeeCounts = await roleRepository.GetEmployeeCountsAsync();
        return roles.Select(role =>
            RoleMapper.ToModel(role, employeeCounts.GetValueOrDefault(role.Id))).ToList();
    }

    public async Task<int> GetIdByNameAsync(string name)
    {
        var role = await roleRepository.GetByNameAsync(name)
            ?? throw new NotFoundException($"Role '{name}' was not found.");

        return role.Id;
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

        var permission = await FindPermissionAsync(model);

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

        var permission = await FindPermissionAsync(model);
        var rolePermission = role.Permissions.FirstOrDefault(p => p.Id == permission.Id)
            ?? throw new NotFoundException(
                $"Role '{role.Name}' does not have permission '{permission.Name}'.");

        role.Permissions.Remove(rolePermission);
        await roleRepository.UpdateAsync(role);
    }

    public async Task<List<RoleEmployeeModel>> GetEmployeesAsync(int roleId)
    {
        var role = await roleRepository.GetWithEmployeesAsync(roleId)
            ?? throw new NotFoundException($"Role with id {roleId} was not found.");

        return role.Employees
            .OrderBy(employee => employee.LastName)
            .ThenBy(employee => employee.FirstName)
            .Select(employee => new RoleEmployeeModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email
            })
            .ToList();
    }

    public async Task AddEmployeeAsync(int roleId, int employeeId)
    {
        var role = await roleRepository.GetWithEmployeesAsync(roleId)
            ?? throw new NotFoundException($"Role with id {roleId} was not found.");
        var employee = await employeeRepository.GetByIdAsync(employeeId)
            ?? throw new NotFoundException($"Employee with id {employeeId} was not found.");

        if (role.Employees.Any(item => item.Id == employeeId))
            throw new ConflictException("This employee already has the role.");

        role.Employees.Add(employee);
        await roleRepository.UpdateAsync(role);
    }

    public async Task RemoveEmployeeAsync(int roleId, int employeeId)
    {
        var role = await roleRepository.GetWithEmployeesAsync(roleId)
            ?? throw new NotFoundException($"Role with id {roleId} was not found.");
        var employee = role.Employees.FirstOrDefault(item => item.Id == employeeId)
            ?? throw new NotFoundException("This employee does not have the role.");

        role.Employees.Remove(employee);
        await roleRepository.UpdateAsync(role);
    }

    private async Task<PermissionEntity> FindPermissionAsync(RolePermissionModel model)
    {
        PermissionEntity? permission = null;

        if (model.PermissionId > 0)
        {
            permission = await permissionRepository.GetByIdAsync(model.PermissionId.Value)
                         ?? throw new NotFoundException($"Permission with id {model.PermissionId} was not found.");
        }

        if (!string.IsNullOrWhiteSpace(model.PermissionName))
        {
            var byName = await permissionRepository.GetByNameAsync(model.PermissionName)
                         ?? throw new NotFoundException($"Permission '{model.PermissionName}' was not found.");

            if (permission != null && permission.Id != byName.Id)
            {
                throw new ConflictException("Permission id and permission name do not match.");
            }

            permission = byName;
        }

        return permission!;
    }
}
