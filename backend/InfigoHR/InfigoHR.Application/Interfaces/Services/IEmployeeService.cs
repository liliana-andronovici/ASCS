using InfigoHR.Application.Common;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Application.QueryParameters.Employees;

namespace InfigoHR.Application.Interfaces.Services;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeModel>> GetAllAsync(EmployeeParameters query);
    Task<EmployeeModel> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateEmployeeModel model);
    Task UpdateAsync(UpdateEmployeeModel model);
    Task DeleteAsync(int id);
    Task<byte[]> ExportCsvAsync();
    Task<ImportResultModel> ImportCsvAsync(Stream csvStream);
}