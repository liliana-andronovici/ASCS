using InfigoHR.Application.Common;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/employees")]
public sealed class EmployeeController(IEmployeeService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeModel>>> GetAll([FromQuery] EmployeeParameters query)
    {
        return Ok(await service.GetAllAsync(query));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeModel>> GetById(int id)
    {
        return Ok(await service.GetByIdAsync(id));
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