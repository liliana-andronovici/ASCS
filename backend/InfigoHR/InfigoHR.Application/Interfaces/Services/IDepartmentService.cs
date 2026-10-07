using InfigoHR.Application.Common;
using InfigoHR.Application.Models.Departments;
using InfigoHR.Application.QueryParameters.Departments;

namespace InfigoHR.Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentModel>> GetAllAsync();
    Task<PagedResult<DepartmentModel>> GetPagedAsync(DepartmentParameters paginationParams);
    Task<DepartmentModel> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateDepartmentModel model);
    Task UpdateAsync(UpdateDepartmentModel model);
    Task DeleteAsync(int id);
}
