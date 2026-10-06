using InfigoHR.Application.Common;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Employees;
using Microsoft.AspNetCore.Mvc;
using InfigoHR.Application.QueryParameters.Employees;
using Microsoft.AspNetCore.Authorization;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/employees")]
public class EmployeeController(IEmployeeService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeModel>>> GetAll([FromQuery] EmployeeParameters query)
    {
        return Ok(await service.GetAllAsync(query));
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