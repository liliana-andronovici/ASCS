using InfigoHR.Application.Models.Departments;

namespace InfigoHR.Application.Interfaces;

public interface IDepartmentService
{
    // changed: Async suffix added
    Task<IEnumerable<DepartmentModel>> GetAllAsync();
    Task<DepartmentModel> GetByIdAsync(int id);
    Task<DepartmentModel> CreateAsync(CreateDepartmentModel model);
    Task<DepartmentModel> UpdateAsync(int id, UpdateDepartmentModel model);
    Task DeleteAsync(int id);
}
