using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Roles;
using InfigoHR.Authentication.Authorization;
using InfigoHR.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[HasPermission(Permission.ManageRoles)]
[Route("api/[controller]")]
public class RolesController(IRoleService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleModel model)
    {
        var id = await service.CreateAsync(model);
        return Ok(new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateRoleModel model)
    {
        await service.UpdateAsync(new UpdateRoleModel { Id = id, Name = model.Name });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("{id:int}/permissions")]
    public async Task<IActionResult> SetPermissions(int id, [FromBody] List<string> permissions)
    {
        await service.SetPermissionsAsync(new SetRolePermissionsModel
        {
            RoleId = id,
            Permissions = permissions
        });
        return NoContent();
    }

    [HttpGet("{id:int}/employees")]
    public async Task<IActionResult> GetEmployees(int id) =>
        Ok(await service.GetEmployeesAsync(id));

    [HttpPost("{id:int}/employees/{employeeId:int}")]
    public async Task<IActionResult> AddEmployee(int id, int employeeId)
    {
        await service.AddEmployeeAsync(id, employeeId);
        return NoContent();
    }

    [HttpDelete("{id:int}/employees/{employeeId:int}")]
    public async Task<IActionResult> RemoveEmployee(int id, int employeeId)
    {
        await service.RemoveEmployeeAsync(id, employeeId);
        return NoContent();
    }
}
