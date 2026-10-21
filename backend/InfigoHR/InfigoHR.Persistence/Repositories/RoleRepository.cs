using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class RoleRepository : GenericRepository<RoleEntity>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<RoleEntity?> GetWithPermissionsAsync(int id) =>
        await Entities.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<RoleEntity>> GetAllWithPermissionsAsync() =>
        await Entities.AsNoTracking().Include(r => r.Permissions).OrderBy(r => r.Id).ToListAsync();

    public async Task<RoleEntity?> GetByNameAsync(string name) =>
        await Entities.AsNoTracking().FirstOrDefaultAsync(r => r.Name == name);
}
