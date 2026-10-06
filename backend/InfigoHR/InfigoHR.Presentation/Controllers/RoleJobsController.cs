using Hangfire;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Roles;
using InfigoHR.Presentation.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RoleJobsController(IBackgroundJobClient jobs, IRoleService roleService) : ControllerBase
{
    // Lista rolurilor cu permisiunile lor (ca sa vezi id-urile). Nu e job.
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await roleService.GetAllAsync());

    [HttpPost]
    public IActionResult CreateRole(CreateRoleModel model)
    {
        var jobId = jobs.Enqueue<RoleJobs>(job => job.CreateAsync(model));
        return Accepted(new { jobId });
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateRole(int id, CreateRoleModel model)
    {
        var update = new UpdateRoleModel { Id = id, Name = model.Name };
        var jobId = jobs.Enqueue<RoleJobs>(job => job.UpdateAsync(update));
        return Accepted(new { jobId });
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteRole(int id)
    {
        var jobId = jobs.Enqueue<RoleJobs>(job => job.DeleteAsync(id));
        return Accepted(new { jobId });
    }

    // Inlocuieste toate permisiunile rolului cu lista primita.
    [HttpPut("{id:int}/permissions")]
    public IActionResult SetPermissions(int id, List<string> permissions)
    {
        var model = new SetRolePermissionsModel { RoleId = id, Permissions = permissions };
        var jobId = jobs.Enqueue<RoleJobs>(job => job.SetPermissionsAsync(model));
        return Accepted(new { jobId });
    }

    [HttpPost("{id:int}/permissions/{permissionName}")]
    public IActionResult AddPermission(int id, string permissionName)
    {
        var model = new RolePermissionModel { RoleId = id, PermissionName = permissionName };
        var jobId = jobs.Enqueue<RoleJobs>(job => job.AddPermissionAsync(model));
        return Accepted(new { jobId });
    }

    [HttpDelete("{id:int}/permissions/{permissionName}")]
    public IActionResult RemovePermission(int id, string permissionName)
    {
        var model = new RolePermissionModel { RoleId = id, PermissionName = permissionName };
        var jobId = jobs.Enqueue<RoleJobs>(job => job.RemovePermissionAsync(model));
        return Accepted(new { jobId });
    }
}
