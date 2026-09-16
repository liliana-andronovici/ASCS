using InfigoHR.Application.Models;

namespace InfigoHR.Application.Interfaces;

public interface IEmployeeService
{
    Task<List<EmployeeModel>> GetAllAsync();
    Task<EmployeeModel> GetByIdAsync(int id);
    Task<EmployeeModel> CreateAsync(CreateEmployeeModel dto);
    Task UpdateAsync(int id, UpdateEmployeeModel dto);
    Task DeleteAsync(int id);
}