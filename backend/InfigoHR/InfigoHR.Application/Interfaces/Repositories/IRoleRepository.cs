using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IRoleRepository : IGenericRepository<RoleEntity>
{
    Task<RoleEntity?> GetWithPermissionsAsync(int id);
    Task<List<RoleEntity>> GetAllWithPermissionsAsync();
    Task<RoleEntity?> GetByNameAsync(string name);
}
