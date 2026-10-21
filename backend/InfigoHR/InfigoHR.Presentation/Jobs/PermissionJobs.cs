using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Permissions;
using Hangfire.MissionControl;

namespace InfigoHR.Presentation.Jobs;

[MissionLauncher(CategoryName = "Permissions")]
public class PermissionJobs(IPermissionService permissionService)
{
    [Mission(Name = "Get permission id")]
    public async Task<int> GetIdByNameAsync(string name) => await permissionService.GetIdByNameAsync(name);

    [Mission(Name = "Create permission")]
    public async Task CreateAsync(CreatePermissionModel model) => await permissionService.CreateAsync(model);

    [Mission(Name = "Update permission")]
    public async Task UpdateAsync(UpdatePermissionModel model) => await permissionService.UpdateAsync(model);

    [Mission(Name = "Delete permission")]
    public async Task DeleteAsync(int id) => await permissionService.DeleteAsync(id);
}
