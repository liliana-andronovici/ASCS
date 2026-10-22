using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.Departments;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Application.Common;
using InfigoHR.Application.QueryParameters.Departments;

namespace InfigoHR.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IValidator<CreateDepartmentModel> _createValidator;
    private readonly IValidator<UpdateDepartmentModel> _updateValidator;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        IValidator<CreateDepartmentModel> createValidator,
        IValidator<UpdateDepartmentModel> updateValidator)
    {
        _departmentRepository = departmentRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<DepartmentModel>> GetAllAsync()
    {
        var departments = await _departmentRepository.GetAllAsync();
        return departments.Select(DepartmentMapper.ToModel);
    }
    
    public async Task<PagedResult<DepartmentModel>> GetPagedAsync(DepartmentParameters paginationParams)
    {
        var pagedEntities = await _departmentRepository.SearchAsync(paginationParams);
        return pagedEntities.ToPagedResult(DepartmentMapper.ToModel);
    }

    public async Task<DepartmentModel> GetByIdAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department is null)
        {
            throw new NotFoundException($"Department with id {id} was not found.");
        }

        return DepartmentMapper.ToModel(department);
    }

    public async Task<int> CreateAsync(CreateDepartmentModel model)
    {
        await _createValidator.ValidateAndThrowAsync(model);

        var department = DepartmentMapper.ToEntity(model);
        await _departmentRepository.AddAsync(department);

        return department.Id;
    }

    public async Task UpdateAsync(UpdateDepartmentModel model)
    {
        await _updateValidator.ValidateAndThrowAsync(model);

        var department = await _departmentRepository.GetByIdAsync(model.Id);

        if (department is null)
        {
            throw new NotFoundException($"Department with id {model.Id} was not found.");
        }

        DepartmentMapper.ToEntity(model, department);
        await _departmentRepository.UpdateAsync(department);
    }

    public async Task DeleteAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department is null)
        {
            throw new NotFoundException($"Department with id {id} was not found.");
        }

        await _departmentRepository.DeleteAsync(department);
    }

    public async Task<byte[]> ExportCsvAsync()
    {
        var departments = await _departmentRepository.GetAllAsync();

        var rows = departments
            .OrderBy(department => department.Id)
            .Select(department => new DepartmentCsvModel { Name = department.Name });

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

        var existingNames = (await _departmentRepository.GetAllAsync())
            .Select(department => department.Name.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

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

            DepartmentCsvModel row;
            try
            {
                row = csv.GetRecord<DepartmentCsvModel>();
            }
            catch (CsvHelperException)
            {
                AddSkippedRow(result, rowNumber, "the row could not be read.");
                continue;
            }

            var name = row.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                AddSkippedRow(result, rowNumber, "Name is required.");
                continue;
            }

            if (existingNames.Contains(name))
            {
                AddSkippedRow(result, rowNumber, $"Department \"{name}\" already exists.");
                continue;
            }

            try
            {
                await CreateAsync(new CreateDepartmentModel { Name = name });
                existingNames.Add(name);
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
