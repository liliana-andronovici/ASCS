using InfigoHR.Application.Interfaces.Repositories;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Mappers;
using InfigoHR.Application.Models.Employees;

namespace InfigoHR.Application.Services;

public class OrgChartService(IEmployeeRepository repository) : IOrgChartService
{
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
