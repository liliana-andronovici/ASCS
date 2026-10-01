using InfigoHR.Application.QueryParameters.Departments;
using InfigoHR.Domain.Common;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IDepartmentRepository : IGenericRepository<DepartmentEntity>
{
    Task<PagedList<DepartmentEntity>> SearchAsync(DepartmentParameters paginationParams);
    Task<bool> ExistsAsync(int id);
}