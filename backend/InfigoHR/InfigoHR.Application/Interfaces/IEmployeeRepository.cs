using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<List<EmployeeEntity>> GetAllAsync();
    Task<EmployeeEntity?> GetByIdAsync(int id);
    Task AddAsync(EmployeeEntity entity);
    Task DeleteAsync(EmployeeEntity entity);
    Task<bool> DepartmentExistsAsync(int id);
    Task<bool> LocationExistsAsync(int id);
    Task SaveChangesAsync();
}