using InfigoHR.Application.Common;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Application.QueryParameters.Employees;
using System.Text.Json;

namespace InfigoHR.Application.Interfaces.Services;

public interface IEmployeeService
{
    Task<EmployeeProfileModel> GetProfileAsync(int id, CancellationToken cancellationToken = default);
    Task<EmployeeProfileModel> PatchAsync(int id, JsonElement body, EmployeeAccess access, CancellationToken cancellationToken = default);
    Task<PagedResult<EmployeeDocumentModel>> GetDocumentsAsync(int id, int page, int pageSize, EmployeeAccess access, CancellationToken cancellationToken = default);
    Task<PagedResult<EmployeeHistoryModel>> GetHistoryAsync(int id, int page, int pageSize, EmployeeAccess access, CancellationToken cancellationToken = default);
    Task<PagedResult<EmployeeModel>> GetAllAsync(EmployeeParameters query);
    Task<EmployeeModel> GetByIdAsync(int id);
    Task<List<EmployeeEquipmentModel>> GetEquipmentAsync(int employeeId);
    Task<int> AddEquipmentAsync(int employeeId, CreateEmployeeEquipmentModel model);
    Task<int> CreateAsync(CreateEmployeeModel model);
    Task UpdateAsync(UpdateEmployeeModel model);
    Task DeleteAsync(int id);
    Task<byte[]> ExportCsvAsync();
    Task<ImportResultModel> ImportCsvAsync(Stream csvStream);
}
