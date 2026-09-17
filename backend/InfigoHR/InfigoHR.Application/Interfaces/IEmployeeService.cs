using InfigoHR.Application.Models;

namespace InfigoHR.Application.Interfaces.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeModel>> GetAllAsync();
    Task<EmployeeModel> GetByIdAsync(int id);
    Task<EmployeeModel> CreateAsync(CreateEmployeeModel model);
    Task UpdateAsync(UpdateEmployeeModel model);
    Task DeleteAsync(int id);
}