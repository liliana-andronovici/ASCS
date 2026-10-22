using FluentValidation;
using System.Text.Json;
using InfigoHR.Application.Common;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Application.Patching;
using InfigoHR.Application.QueryParameters.Employees;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Services;

public class EmployeeService(
    IEmployeeRepository repository,
    IEquipmentRepository equipmentRepository,
    IValidator<CreateEmployeeModel> createValidator,
    IValidator<UpdateEmployeeModel> updateValidator,
    IValidator<CreateEmployeeEquipmentModel> equipmentValidator,
    IValidator<EmployeeEntity> profileValidator) : IEmployeeService
{
    public async Task<EmployeeProfileModel> GetProfileAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetProfileAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Employee with id {id} was not found.");
        var profile = EmployeeMapper.ToProfileModel(employee);
        profile.DirectReports = await repository.GetDirectReportsAsync(id, cancellationToken);
        return profile;
    }

    public async Task<EmployeeProfileModel> PatchAsync(int id, JsonElement body, EmployeeAccess access,
        CancellationToken cancellationToken = default)
    {
        if (!access.CanManageEmployees)
            throw new ForbiddenException("Managing employees requires permission.");
        var expectedVersion = EmployeePatch.GetVersion(body);
        await repository.UpdateProfileAsync(id, expectedVersion, access.EmployeeId, async (employee, token) =>
        {
            EmployeePatch.Apply(body, employee);
            await profileValidator.ValidateAndThrowAsync(employee, token);
        }, cancellationToken);
        return await GetProfileAsync(id, cancellationToken);
    }

    public async Task<PagedResult<EmployeeDocumentModel>> GetDocumentsAsync(int id, int page, int pageSize,
        EmployeeAccess access, CancellationToken cancellationToken = default)
    {
        await CheckRecordsAccessAsync(id, page, pageSize, access, cancellationToken);
        return await repository.GetDocumentsAsync(id, page, pageSize, cancellationToken);
    }

    public async Task<PagedResult<EmployeeHistoryModel>> GetHistoryAsync(int id, int page, int pageSize,
        EmployeeAccess access, CancellationToken cancellationToken = default)
    {
        await CheckRecordsAccessAsync(id, page, pageSize, access, cancellationToken);
        return await repository.GetHistoryAsync(id, page, pageSize, cancellationToken);
    }

    private async Task CheckRecordsAccessAsync(int id, int page, int pageSize, EmployeeAccess access,
        CancellationToken cancellationToken)
    {
        if (!access.CanManageEmployees && access.EmployeeId != id)
            throw new ForbiddenException("This employee's records are private.");
        if (page < 1 || pageSize is < 1 or > 100)
            throw new InfigoHR.Application.Exceptions.ValidationException("Page must be positive and page size must be between 1 and 100.");
        if (await repository.GetProfileAsync(id, cancellationToken) is null)
            throw new NotFoundException($"Employee with id {id} was not found.");
    }

    public async Task<PagedResult<EmployeeModel>> GetAllAsync(EmployeeParameters query)
    {
        var pagedList = await repository.SearchAsync(query);
        return pagedList.ToPagedResult(EmployeeMapper.ToModel);
    }

    public async Task<EmployeeModel> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"Employee with id {id} was not found.");
        return EmployeeMapper.ToModel(entity);
    }

    public async Task<List<EmployeeEquipmentModel>> GetEquipmentAsync(int employeeId)
    {
        if (await repository.GetByIdAsync(employeeId) is null)
            throw new NotFoundException($"Employee with id {employeeId} was not found.");

        var equipment = await repository.GetEquipmentAsync(employeeId);
        return equipment.Select(item => new EmployeeEquipmentModel
        {
            Id = item.Id,
            EquipmentId = item.EquipmentId,
            Name = item.Equipment.Name,
            Code = item.Equipment.Code,
            TypeName = item.Equipment.EquipmentType.Name,
            AssignedOnUtc = item.AssignedOnUtc,
            ReturnedOnUtc = item.ReturnedOnUtc
        }).ToList();
    }

    public async Task<int> AddEquipmentAsync(int employeeId, CreateEmployeeEquipmentModel model)
    {
        await equipmentValidator.ValidateAndThrowAsync(model);

        if (await repository.GetByIdAsync(employeeId) is null)
            throw new NotFoundException($"Employee with id {employeeId} was not found.");

        if (await equipmentRepository.GetByIdAsync(model.EquipmentId) is null)
            throw new NotFoundException($"Asset with id {model.EquipmentId} was not found.");

        return await repository.AddEquipmentAsync(new EmployeeEquipmentEntity
        {
            EmployeeId = employeeId,
            EquipmentId = model.EquipmentId,
            AssignedOnUtc = DateTime.SpecifyKind(model.AssignedOn.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
        });
    }

    public async Task<int> CreateAsync(CreateEmployeeModel model)
    {
        await createValidator.ValidateAndThrowAsync(model);

        var entity = EmployeeMapper.ToEntity(model);
        await repository.AddAsync(entity);
        
        return entity.Id;
    }

    public async Task UpdateAsync(UpdateEmployeeModel model)
    {
        await updateValidator.ValidateAndThrowAsync(model);

        var entity = await repository.GetByIdAsync(model.Id)
                     ?? throw new NotFoundException($"Employee with id {model.Id} was not found.");

        EmployeeMapper.ToEntity(model, entity);
        await repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"Employee with id {id} was not found.");
        await repository.DeleteAsync(entity);
    }
}
