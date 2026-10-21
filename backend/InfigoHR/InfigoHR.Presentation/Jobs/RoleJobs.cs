using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Roles;
using Hangfire.MissionControl;

namespace InfigoHR.Presentation.Jobs;

[MissionLauncher(CategoryName = "Roles")]
public class RoleJobs(IRoleService roleService)
{
    [Mission(Name = "Get role id")]
    public async Task<int> GetIdByNameAsync(string name) => await roleService.GetIdByNameAsync(name);

    [Mission(Name = "Create role")]
    public async Task CreateAsync(CreateRoleModel model) => await roleService.CreateAsync(model);

    [Mission(Name = "Update role")]
    public async Task UpdateAsync(UpdateRoleModel model) => await roleService.UpdateAsync(model);

    [Mission(Name = "Delete role")]
    public async Task DeleteAsync(int id) => await roleService.DeleteAsync(id);

    public async Task SetPermissionsAsync(SetRolePermissionsModel model) =>
        await roleService.SetPermissionsAsync(model);

    [Mission(Name = "Set role permissions")]
    public Task SetPermissionsMissionAsync(int roleId, string permissions) =>
        roleService.SetPermissionsAsync(new SetRolePermissionsModel
        {
            RoleId = roleId,
            Permissions = permissions
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList()
        });

    [Mission(Name = "Add role permission")]
    public async Task AddPermissionAsync(RolePermissionModel model) =>
        await roleService.AddPermissionAsync(model);

    [Mission(Name = "Remove role permission")]
    public async Task RemovePermissionAsync(RolePermissionModel model) =>
        await roleService.RemovePermissionAsync(model);
}
