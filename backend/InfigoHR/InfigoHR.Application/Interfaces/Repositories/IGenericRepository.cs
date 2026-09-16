using InfigoHR.Domain.Common;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task AddAsync(TEntity entity);
    void Remove(TEntity entity);
    Task SaveChangesAsync();
}
