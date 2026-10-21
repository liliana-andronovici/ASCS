using Hangfire;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models.Permissions;
using InfigoHR.Presentation.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PermissionJobsController(IBackgroundJobClient jobs, IPermissionService permissionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await permissionService.GetAllAsync());

    [HttpPost]
    public IActionResult CreatePermission(CreatePermissionModel model)
    {
        var jobId = jobs.Enqueue<PermissionJobs>(job => job.CreateAsync(model));
        return Accepted(new { jobId });
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdatePermission(int id, CreatePermissionModel model)
    {
        var update = new UpdatePermissionModel { Id = id, Name = model.Name };
        var jobId = jobs.Enqueue<PermissionJobs>(job => job.UpdateAsync(update));
        return Accepted(new { jobId });
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeletePermission(int id)
    {
        var jobId = jobs.Enqueue<PermissionJobs>(job => job.DeleteAsync(id));
        return Accepted(new { jobId });
    }
}
