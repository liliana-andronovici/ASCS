using InfigoHR.Application.Common;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface ILocationRepository : IGenericRepository<LocationEntity>
{
    Task<PagedResult<LocationEntity>> GetPagedAsync(PaginationParams paginationParams);
}