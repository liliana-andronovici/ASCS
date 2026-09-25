using FluentValidation;
using InfigoHR.Application.Common;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models;
using InfigoHR.Application.QueryParameters.Employees;
using System.Text;

namespace InfigoHR.Application.Services;

public class EmployeeService(
    IEmployeeRepository repository,
    IValidator<CreateEmployeeModel> createValidator,
    IValidator<UpdateEmployeeModel> updateValidator) : IEmployeeService
{
    public async Task<PagedResult<EmployeeModel>> GetAllAsync(EmployeeParameters query)
    {
        var pagedList = await repository.SearchAsync(query);
        return pagedList.ToPagedResult(EmployeeMapper.ToModel);
    }

    public async Task<byte[]> ExportCsvAsync(EmployeeParameters query)
    {
        var entities = await repository.SearchAllAsync(query);

        var csv = new StringBuilder();
        csv.AppendLine("Name,Title,Email,Status,Started");

        foreach (var entity in entities)
        {
            var name = EscapeCsvField($"{entity.FirstName} {entity.LastName}");
            var title = EscapeCsvField(entity.Title);
            var email = EscapeCsvField(entity.Email);
            var status = EscapeCsvField(entity.Status.ToString());
            var started = EscapeCsvField(entity.StartedOnUtc.ToString("yyyy-MM-dd"));
            csv.AppendLine($"{name},{title},{email},{status},{started}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private static string EscapeCsvField(string value) =>
        $"\"{value.Replace("\"", "\"\"")}\"";

    public async Task<EmployeeModel> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"Employee with id {id} was not found.");
        return EmployeeMapper.ToModel(entity);
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