using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Departments;
using InfigoHR.Authentication.Authorization;
using InfigoHR.Domain.Enums;
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

    [HasPermission(Permission.ReadDepartment)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetAll()
    {
        var departments = await _departmentService.GetAllAsync();
        return Ok(departments);
    }
    
    [HasPermission(Permission.ReadDepartment)]
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<DepartmentModel>>> GetPaged(
        [FromQuery] DepartmentParameters paginationParams)
    {
        var pagedDepartments = await _departmentService.GetPagedAsync(paginationParams);
        return Ok(pagedDepartments);
    }

    [HasPermission(Permission.ReadDepartment)]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentModel>> GetById(int id)
    {
        var department = await _departmentService.GetByIdAsync(id);
        return Ok(department);
    }

    [HasPermission(Permission.ManageDepartment)]
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateDepartmentModel model)
    {
        var id = await _departmentService.CreateAsync(model);
        return Ok(id);
    }

    [HasPermission(Permission.ManageDepartment)]
    [HttpPut]
    public async Task<IActionResult> Update(UpdateDepartmentModel model)
    {
        await _departmentService.UpdateAsync(model);
        return NoContent();
    }

    [HasPermission(Permission.ManageDepartment)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _departmentService.DeleteAsync(id);
        return NoContent();
    }
}
