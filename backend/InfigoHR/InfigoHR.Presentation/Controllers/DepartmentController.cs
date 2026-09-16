using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Departments;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetAll()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentModel>> GetById(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        return Ok(department);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDepartmentModel model)
    {
        await _departmentService.CreateAsync(model);
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDepartmentModel model)
    {
        model.Id = id;
        await _departmentService.UpdateAsync(model);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _departmentService.DeleteAsync(id);
        return NoContent();
    }
}
