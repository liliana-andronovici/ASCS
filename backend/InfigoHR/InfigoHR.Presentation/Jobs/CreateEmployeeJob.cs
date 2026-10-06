using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Employees;

namespace InfigoHR.Presentation.Jobs;

public class CreateEmployeeJob(IEmployeeService employeeService)
{
    public async Task ExecuteAsync(CreateEmployeeModel employee)
    {
        await employeeService.CreateAsync(employee);
    }
}