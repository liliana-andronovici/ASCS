using FluentValidation;
using InfigoHR.Application.Common;
using InfigoHR.Application.Exceptions;
using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Application.QueryParameters.Employees;

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
    
    public async Task<IEnumerable<OrgChartEmployeeModel>> GetOrgChartAsync()
    {
        var employees = (await repository.GetAllAsync()).ToList();

        var employeeModels = employees.ToDictionary(
            employee => employee.Id,
            EmployeeMapper.ToOrgChartModel);

        var roots = new List<OrgChartEmployeeModel>();

        foreach (var employee in employees)
        {
            var employeeModel = employeeModels[employee.Id];

            if (employee.ManagerId.HasValue &&
                employeeModels.TryGetValue(employee.ManagerId.Value, out var manager))
            {
                manager.DirectReports.Add(employeeModel);
            }
            else
            {
                roots.Add(employeeModel);
            }
        }

        return roots;
    }
    
    
}