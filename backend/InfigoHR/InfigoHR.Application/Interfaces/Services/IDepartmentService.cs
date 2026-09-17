using InfigoHR.Application.Models.Departments;

namespace InfigoHR.Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentModel>> GetAllAsync();
    Task<DepartmentModel> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateDepartmentModel model);
    Task UpdateAsync(UpdateDepartmentModel model);
    Task DeleteAsync(int id);
}
