using InfigoHR.Application.Common;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models;
using Microsoft.AspNetCore.Mvc;
using InfigoHR.Application.QueryParameters.Employees;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeeController(IEmployeeService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeModel>>> GetAll([FromQuery] EmployeeParameters query)
    {
        return Ok(await service.GetAllAsync(query));
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] EmployeeParameters query)
    {
        var csvBytes = await service.ExportCsvAsync(query);
        return File(csvBytes, "text/csv", "people.csv");
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeModel>> GetById(int id)
    {
        var employee = await service.GetByIdAsync(id);
        return Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateEmployeeModel model)
    {
        var id = await service.CreateAsync(model);
        return Ok(id);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateEmployeeModel model)
    {
        await service.UpdateAsync(model);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}