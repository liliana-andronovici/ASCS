using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IEmployeeRepository : IGenericRepository<EmployeeEntity>
{
    Task<bool> EmailExistsAsync(string email);
    Task<bool> DepartmentExistsAsync(int id);
    Task<bool> LocationExistsAsync(int id);
}