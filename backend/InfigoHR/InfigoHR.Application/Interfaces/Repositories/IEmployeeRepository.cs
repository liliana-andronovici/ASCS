using InfigoHR.Application.QueryParameters.Employees;
using InfigoHR.Domain.Common;
using InfigoHR.Domain.Entities;
using InfigoHR.Application.Models;
using InfigoHR.Application.Common;
using InfigoHR.Application.Models.Employees;

namespace InfigoHR.Application.Interfaces.Repositories;

public interface IEmployeeRepository : IGenericRepository<EmployeeEntity>
{
    Task<EmployeeEntity?> GetProfileAsync(int id, CancellationToken cancellationToken = default);
    Task<List<EmployeeMainDataModel>> GetDirectReportsAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateProfileAsync(int id, long expectedVersion, int actorId, Func<EmployeeEntity, CancellationToken, Task> update, CancellationToken cancellationToken = default);
    Task<PagedResult<EmployeeDocumentModel>> GetDocumentsAsync(int id, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<EmployeeHistoryModel>> GetHistoryAsync(int id, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<EmployeeEntity?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<PagedList<EmployeeEntity>> SearchAsync(EmployeeParameters query);

    Task<List<EmployeeEntity>> GetAllWithDetailsAsync();
    Task<List<EmployeeEquipmentEntity>> GetEquipmentAsync(int employeeId);
    Task<int> AddEquipmentAsync(EmployeeEquipmentEntity equipment);
    Task<List<OnboardingEmployeeModel>> GetAllWithDepartmentAsync();
    Task<bool> ExistsAsync(int id);

    Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default);
}
