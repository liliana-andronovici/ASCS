using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IPermissionRepository : IGenericRepository<PermissionEntity>
{
    Task<PermissionEntity?> GetByNameAsync(string name);
}
