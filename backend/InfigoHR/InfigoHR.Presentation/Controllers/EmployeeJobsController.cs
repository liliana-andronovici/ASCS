using Hangfire;
using InfigoHR.Application.Models.Employees;
using InfigoHR.Presentation.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class EmployeeJobsController(IBackgroundJobClient jobs) : ControllerBase
{
    [HttpPost]
    public IActionResult CreateEmployee(CreateEmployeeModel model)
    {
        var jobId = jobs.Enqueue<CreateEmployeeJob>(
            job => job.ExecuteAsync(model));

        return Accepted(new { jobId });
    }
}