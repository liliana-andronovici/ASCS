using InfigoHR.Application.Common;
using InfigoHR.Application.Models;

namespace InfigoHR.Application.Interfaces.Services;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeModel>> GetAllAsync(EmployeeParameters query);
    Task<EmployeeModel> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateEmployeeModel model);
    Task UpdateAsync(UpdateEmployeeModel model);
    Task DeleteAsync(int id);
}