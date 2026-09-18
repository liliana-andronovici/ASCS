using InfigoHR.Application.Common;
using InfigoHR.Domain.Entities;
using InfigoHR.Domain.Common;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface ILocationRepository : IGenericRepository<LocationEntity>
{
    Task<PagedList<LocationEntity>> GetPagedAsync(PaginationParams paginationParams);
}