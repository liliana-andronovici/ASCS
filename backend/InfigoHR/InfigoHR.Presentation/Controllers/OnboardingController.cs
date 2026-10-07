using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OnboardingController(IOnboardingService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<OnboardingSummaryModel>>> GetAll() =>
        Ok(await service.GetOnboardingSummariesAsync());

    [HttpGet("{employeeId:int}")]
    public async Task<ActionResult<OnboardingChecklistModel>> GetChecklistFor(int employeeId) =>
        Ok(await service.GetChecklistAsync(employeeId));

    [HttpPost("{employeeId:int}/tasks")]
    public async Task<IActionResult> AddTask(int employeeId, AddOnboardingTaskModel model)
    {
        await service.AddTaskAsync(employeeId, model);
        return NoContent();
    }

    [HttpPost("{employeeId:int}/tasks/{taskId:int}/toggle")]
    public async Task<IActionResult> ToggleTask(int employeeId, int taskId)
    {
        await service.ToggleTaskAsync(employeeId, taskId);
        return NoContent();
    }
}
