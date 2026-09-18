using InfigoHR.Application.Common;
using InfigoHR.Domain.Common;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IEmployeeRepository : IGenericRepository<EmployeeEntity>
{
    Task<PagedList<EmployeeEntity>> SearchAsync(EmployeeParameters query);
    Task<bool> EmailExistsAsync(string email);
}