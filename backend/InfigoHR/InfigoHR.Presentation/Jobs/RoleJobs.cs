using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Roles;

namespace InfigoHR.Presentation.Jobs;

public class RoleJobs(IRoleService roleService)
{
    public async Task CreateAsync(CreateRoleModel model) => await roleService.CreateAsync(model);

    public async Task UpdateAsync(UpdateRoleModel model) => await roleService.UpdateAsync(model);

    public async Task DeleteAsync(int id) => await roleService.DeleteAsync(id);

    public async Task SetPermissionsAsync(SetRolePermissionsModel model) =>
        await roleService.SetPermissionsAsync(model);

    public async Task AddPermissionAsync(RolePermissionModel model) =>
        await roleService.AddPermissionAsync(model);

    public async Task RemovePermissionAsync(RolePermissionModel model) =>
        await roleService.RemovePermissionAsync(model);
}
