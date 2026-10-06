using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Permissions;

namespace InfigoHR.Presentation.Jobs;

public class PermissionJobs(IPermissionService permissionService)
{
    public async Task CreateAsync(CreatePermissionModel model) => await permissionService.CreateAsync(model);

    public async Task UpdateAsync(UpdatePermissionModel model) => await permissionService.UpdateAsync(model);

    public async Task DeleteAsync(int id) => await permissionService.DeleteAsync(id);
}
