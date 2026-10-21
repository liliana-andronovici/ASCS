using InfigoHR.Application.Models.Roles;

namespace InfigoHR.Application.Interfaces.Services;

public interface IRoleService
{
    Task<List<RoleModel>> GetAllAsync();
    Task<int> GetIdByNameAsync(string name);
    Task<int> CreateAsync(CreateRoleModel model);
    Task UpdateAsync(UpdateRoleModel model);
    Task DeleteAsync(int id);
    Task SetPermissionsAsync(SetRolePermissionsModel model);
    Task AddPermissionAsync(RolePermissionModel model);
    Task RemovePermissionAsync(RolePermissionModel model);
}
