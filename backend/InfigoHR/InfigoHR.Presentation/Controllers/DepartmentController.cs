using FluentValidation;
using InfigoHR.Application.Interfaces;
using InfigoHR.Application.Models.Departments;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    private readonly IValidator<CreateDepartmentModel> _createValidator;
    private readonly IValidator<UpdateDepartmentModel> _updateValidator;

    public DepartmentController(
        IDepartmentService departmentService,
        IValidator<CreateDepartmentModel> createValidator,
        IValidator<UpdateDepartmentModel> updateValidator)
    {
        _departmentService = departmentService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetAll()
    {
        var departments = await _departmentService.GetAllAsync(); // changed: Async suffix
        return Ok(departments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentModel>> GetById(int id)
    {
        // changed: NotFound is now handled with custom exception + middleware
        var department = await _departmentService.GetByIdAsync(id);
        return Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentModel>> Create(CreateDepartmentModel model)
    {
        // changed: FluentValidation used
        await _createValidator.ValidateAndThrowAsync(model);

        var department = await _departmentService.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DepartmentModel>> Update(int id, UpdateDepartmentModel model)
    {
        // changed: FluentValidation used
        await _updateValidator.ValidateAndThrowAsync(model);

        var department = await _departmentService.UpdateAsync(id, model);
        return Ok(department);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _departmentService.DeleteAsync(id);
        return NoContent();
    }
}
