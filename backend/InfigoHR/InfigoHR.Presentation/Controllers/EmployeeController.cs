using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/employees")]
public sealed class EmployeeController(IEmployeeService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetAll() =>
        Ok(await service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeModel>> GetById(int id) =>
        Ok(await service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeModel model)
    {
        var created = await service.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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