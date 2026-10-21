using Ardalis.GuardClauses;
using System.Linq.Expressions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace InfigoHR.Persistence.Repositories;

public class GenericRepository<TEntity>(AppDbContext dbContext) : IGenericRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly AppDbContext Context = dbContext;
    protected readonly DbSet<TEntity> Entities = dbContext.Set<TEntity>();

    public async Task<IEnumerable<TEntity>> GetAllAsync() =>
        await Entities.AsNoTracking().ToListAsync();

    public async Task<TEntity?> GetByIdAsync(int id) =>
        await Entities.FirstOrDefaultAsync(e => e.Id == id);

    public async Task AddAsync(TEntity entity)
    {
        Guard.Against.Null(entity, nameof(entity));

        await Entities.AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        Guard.Against.Null(entity, nameof(entity));

        entity.UpdatedOnUtc = DateTime.UtcNow;
        Entities.Update(entity);
        await Context.SaveChangesAsync();
    }

    protected Task<int> UpdateAsync(
        Expression<Func<TEntity, bool>> predicate,
        Action<UpdateSettersBuilder<TEntity>> setProperties,
        CancellationToken cancellationToken = default)
    {
        return Entities
            .Where(predicate)
            .ExecuteUpdateAsync(setProperties, cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity)
    {
        Guard.Against.Null(entity, nameof(entity));

        Entities.Remove(entity);
        await Context.SaveChangesAsync();
    }
}
