using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly AppDbContext DbContext;
    protected readonly DbSet<TEntity> Entities;

    public GenericRepository(AppDbContext dbContext)
    {
        DbContext = dbContext;
        Entities = dbContext.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await Entities.AsNoTracking().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await Entities.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task AddAsync(TEntity entity)
    {
        await Entities.AddAsync(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        entity.UpdatedOnUtc = DateTime.UtcNow;
        Entities.Update(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        Entities.Remove(entity);
        await DbContext.SaveChangesAsync();
    }
}
