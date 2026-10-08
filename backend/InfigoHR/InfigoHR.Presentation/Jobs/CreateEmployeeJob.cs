using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Employees;
using Hangfire.MissionControl;

namespace InfigoHR.Presentation.Jobs;

[MissionLauncher(CategoryName = "Employees")]
public class CreateEmployeeJob(IEmployeeService employeeService)
{
    [Mission(Name = "Create employee")]
    public async Task ExecuteAsync(CreateEmployeeModel employee)
    {
        await employeeService.CreateAsync(employee);
    }
}
