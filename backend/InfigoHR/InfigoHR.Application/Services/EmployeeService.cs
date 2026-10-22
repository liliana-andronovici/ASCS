using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using InfigoHR.Application.Common;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Application.QueryParameters.Employees;
using InfigoHR.Domain.Entities;

namespace InfigoHR.Application.Services;

public class EmployeeService(
    IEmployeeRepository repository,
    IDepartmentRepository departmentRepository,
    ILocationRepository locationRepository,
    IEquipmentRepository equipmentRepository,
    IValidator<CreateEmployeeModel> createValidator,
    IValidator<UpdateEmployeeModel> updateValidator,
    IValidator<CreateEmployeeEquipmentModel> equipmentValidator) : IEmployeeService
{
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

    public async Task<byte[]> ExportCsvAsync()
    {
        var employees = await repository.GetAllWithDetailsAsync();

        var rows = employees.Select(employee => new EmployeeCsvModel
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Title = employee.Title,
            StartedOnUtc = employee.StartedOnUtc,
            Department = employee.Department?.Name,
            Location = employee.Location?.Name
        });

        using var stream = new MemoryStream();
        await using (var writer = new StreamWriter(stream, leaveOpen: true))
        await using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            await csv.WriteRecordsAsync(rows);
        }

        return stream.ToArray();
    }

    public async Task<ImportResultModel> ImportCsvAsync(Stream csvStream)
    {
        var result = new ImportResultModel();

        var departments = (await departmentRepository.GetAllAsync())
            .GroupBy(department => department.Name.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().Id, StringComparer.OrdinalIgnoreCase);

        var locations = (await locationRepository.GetAllAsync())
            .GroupBy(location => location.Name.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().Id, StringComparer.OrdinalIgnoreCase);

        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        };

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, configuration);

        if (!await csv.ReadAsync())
            return result;

        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            var rowNumber = csv.Parser.Row;

            EmployeeCsvModel row;
            try
            {
                row = csv.GetRecord<EmployeeCsvModel>();
            }
            catch (CsvHelperException)
            {
                AddSkippedRow(result, rowNumber, "the row could not be read, check the values and the date format.");
                continue;
            }

            int? departmentId = null;
            if (!string.IsNullOrWhiteSpace(row.Department))
            {
                if (!departments.TryGetValue(row.Department.Trim(), out var foundDepartmentId))
                {
                    AddSkippedRow(result, rowNumber, $"Department \"{row.Department}\" was not found.");
                    continue;
                }

                departmentId = foundDepartmentId;
            }

            int? locationId = null;
            if (!string.IsNullOrWhiteSpace(row.Location))
            {
                if (!locations.TryGetValue(row.Location.Trim(), out var foundLocationId))
                {
                    AddSkippedRow(result, rowNumber, $"Location \"{row.Location}\" was not found.");
                    continue;
                }

                locationId = foundLocationId;
            }

            var model = new CreateEmployeeModel
            {
                FirstName = row.FirstName,
                LastName = row.LastName,
                Email = row.Email,
                Title = row.Title,
                StartedOnUtc = DateTime.SpecifyKind(row.StartedOnUtc, DateTimeKind.Utc),
                DepartmentId = departmentId,
                LocationId = locationId
            };

            try
            {
                await CreateAsync(model);
                result.Imported++;
            }
            catch (FluentValidation.ValidationException ex)
            {
                AddSkippedRow(result, rowNumber, string.Join(" ", ex.Errors.Select(error => error.ErrorMessage)));
            }
        }

        return result;
    }

    private static void AddSkippedRow(ImportResultModel result, int rowNumber, string reason)
    {
        result.Skipped++;
        result.Errors.Add($"Row {rowNumber}: {reason}");
    }
}
