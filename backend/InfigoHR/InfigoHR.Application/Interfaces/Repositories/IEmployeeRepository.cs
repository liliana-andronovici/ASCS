using InfigoHR.Application.QueryParameters.Employees;
using InfigoHR.Domain.Common;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IEmployeeRepository : IGenericRepository<EmployeeEntity>
{
    Task<EmployeeEntity?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<PagedList<EmployeeEntity>> SearchAsync(EmployeeParameters query);

    Task<List<EmployeeEntity>> GetAllWithDetailsAsync();
    
    Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default);
}