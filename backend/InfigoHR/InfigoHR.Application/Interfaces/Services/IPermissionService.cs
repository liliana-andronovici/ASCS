using InfigoHR.Application.Models.Permissions;

namespace InfigoHR.Application.Interfaces.Services;

public interface IPermissionService
{
    Task<List<PermissionModel>> GetAllAsync();
    Task<int> CreateAsync(CreatePermissionModel model);
    Task UpdateAsync(UpdatePermissionModel model);
    Task DeleteAsync(int id);
}
