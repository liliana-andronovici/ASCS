using System.IdentityModel.Tokens.Jwt;
using InfigoHR.Application.Interfaces.Services;
using InfigoHR.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace InfigoHR.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OnboardingController(IOnboardingService service) : ControllerBase
{
    private const int DevFallbackEmployeeId = 14;

    [HttpGet]
    public async Task<ActionResult<List<OnboardingSummaryModel>>> GetAll() =>
        Ok(await service.GetOnboardingSummariesAsync());

    [HttpGet("checklist")]
    public async Task<ActionResult<OnboardingChecklistModel>> GetChecklist() =>
        Ok(await service.GetChecklistAsync(GetCurrentEmployeeId()));

    [HttpGet("{employeeId:int}")]
    public async Task<ActionResult<OnboardingChecklistModel>> GetChecklistFor(int employeeId) =>
        Ok(await service.GetChecklistAsync(employeeId));

    [HttpPost("{employeeId:int}/tasks")]
    public async Task<IActionResult> AddTask(int employeeId, AddOnboardingTaskModel model)
    {
        await service.AddTaskAsync(employeeId, model);
        return NoContent();
    }

    [HttpPost("tasks/{taskId:int}/toggle")]
    public async Task<IActionResult> ToggleTask(int taskId)
    {
        await service.ToggleTaskAsync(GetCurrentEmployeeId(), taskId);
        return NoContent();
    }

    private int GetCurrentEmployeeId()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        return sub is not null ? int.Parse(sub) : DevFallbackEmployeeId;
    }
}