using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class PermissionRepository : GenericRepository<PermissionEntity>, IPermissionRepository
{
    public PermissionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PermissionEntity?> GetByNameAsync(string name) =>
        await Entities.FirstOrDefaultAsync(p => p.Name == name);
}
