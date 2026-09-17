using Ardalis.GuardClauses;
using InfigoHR.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InfigoHR.Persistence.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext Context;
    protected DbSet<TEntity> Entities => Context.Set<TEntity>();

    public GenericRepository(AppDbContext context)
    {
        Context = context;
    }

    public async Task<List<TEntity>> GetAllAsync()
        => await Entities.ToListAsync();

    public async Task<TEntity?> GetByIdAsync(int id)
        => await Entities.FindAsync(id);

    public async Task AddAsync(TEntity entity)
    {
        Guard.Against.Null(entity, nameof(entity));
        await Entities.AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        Guard.Against.Null(entity, nameof(entity));
        Entities.Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        Guard.Against.Null(entity, nameof(entity));
        Entities.Remove(entity);
        await Context.SaveChangesAsync();
    }
}