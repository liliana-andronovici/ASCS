using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Departments;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using InfigoHR.Application.Common;
using InfigoHR.Application.QueryParameters.Departments;

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
    
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<DepartmentModel>>> GetPaged(
        [FromQuery] DepartmentParameters paginationParams)
    {
        var pagedDepartments = await _departmentService.GetPagedAsync(paginationParams);
        return Ok(pagedDepartments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentModel>> GetById(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        return Ok(department);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var file = await _departmentService.ExportCsvAsync();
        return File(file, "text/csv", "departments.csv");
    }

    [HttpPost("import")]
    public async Task<ActionResult<ImportResultModel>> Import(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new ErrorResponseModel { Message = "Please upload a non-empty CSV file." });

        await using var stream = file.OpenReadStream();
        var result = await _departmentService.ImportCsvAsync(stream);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateDepartmentModel model)
    {
        var id = await _departmentService.CreateAsync(model);
        return Ok(id);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateDepartmentModel model)
    {
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
